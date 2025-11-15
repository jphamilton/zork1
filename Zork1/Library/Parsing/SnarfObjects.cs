using Zork1.Library.Extensions;
using Zork1.Library.Things;

namespace Zork1.Library.Parsing;

public static class Snarf
{
    private record ObjLocByte(Object Object, byte LocByte);
    private static Func<IEnumerable<Object>, string> WhichDoYouMean = (list) => $"Which do you mean, {Display.List(list, true, "or")}?";

    public static bool Objects(Frame frame, Grammar grammar)
    {
        var required = grammar.Required;

        // no objects required
        if (required == 0 && frame.Objects.Count == 0 && frame.IndirectObjects.Count == 0)
        {
            return false;
        }

        // too dark?
        if (!State.Lit && !AvailableInDark(frame, grammar))
        {
            frame.Error = Messages.TooDarkToSee;
            return false;
        }

        // Unresolved Objects
        if (frame.UnresolvedObjects.Count > 0 && !ResolveObjects(frame, grammar, true))
        {
            return false;
        }

        if (frame.UnresolvedIndirectObjects.Count > 0 && !ResolveObjects(frame, grammar, false))
        {
            return false;
        }

        // all/except (BUT-MERGE)
        if ((frame.All || frame.Except) && !ButMerge(frame, grammar))
        {
            return false;
        }

        // check that syntax has required number of noun clauses and try to supply missing objects
        if (!HasRequired(frame, grammar))
        {
            // get locbytes for what we have
            frame.LocBytes = GetLocBytes(frame, grammar);
            return false;
        }

        frame.LocBytes = GetLocBytes(frame, grammar);

        return true;
    }

    private static bool ResolveObjects(Frame frame, Grammar grammar, bool directObject)
    {
        var xLocByte = directObject ? grammar.Object.LocByte : grammar.IndirectObject.LocByte;
        var xGwim = directObject ? grammar.Object.Gwim : grammar.IndirectObject.Gwim;
        var unresolvedObjects = directObject ? frame.UnresolvedObjects : frame.UnresolvedIndirectObjects;
        var target = directObject ? frame.Objects : frame.IndirectObjects;

        foreach (var uObj in unresolvedObjects.ToList())
        {
            List<Object> matched = ResolveByXLocByte(xLocByte, uObj.Objects, xGwim);

            if (matched.Count == 0)
            {
                matched = uObj.Objects;
            }

            if (matched.Count == 1)
            {
                target.Add(matched[0]);
                unresolvedObjects.Remove(uObj);
            }
            else if (matched.Count > 1)
            {
                uObj.Objects = matched;
                frame.Error = WhichDoYouMean(matched);
                return false;
            }
        }

        return true;
    }

    private static bool ButMerge(Frame frame, Grammar grammar)
    {
        var objects = frame.All ? GetObjects(true) : [.. frame.Objects];

        if (!State.Lit)
        {
            objects = SearchList.Top(Player.Instance);
            if (objects.Count == 0)
            {
                frame.Error = Messages.TooDarkToSee;
                return false;
            }
        }

        if (frame.Except)
        {
            objects = [.. objects.Where(x => !frame.ExceptObjects.Contains(x))];

            frame.ExceptObjects = [];

            // EXCEPT completely wiped out object list
            if (objects.Count == 0)
            {
                frame.Error = Messages.NotClear;
                return false;
            }
        }

        var xLocByte = grammar.Object.LocByte;
        var gwim = grammar.Object.Gwim;

        var matched = GetMatches(xLocByte, objects, gwim);

        if (matched.Count == 0)
        {
            frame.Error = Messages.NotClear;
            return false;
        }

        frame.Objects = [.. matched];

        return true;
    }

    private static bool AvailableInDark(Frame frame, Grammar grammar)
    {
        if (frame.UnresolvedObjects.Count > 0)
        {
            ResolveObjects(frame, grammar, true);
        }

        if (frame.UnresolvedIndirectObjects.Count > 0)
        {
            ResolveObjects(frame, grammar, false);
        }

        var available = (List<Object>)[.. frame.Objects, .. frame.IndirectObjects];
        var held = SearchList.Top(Player.Instance);

        return available.All(held.Contains);
    }

    private static bool HasRequired(Frame frame, Grammar grammar)
    {
        var required = grammar.Required;

        // Object required, but none found, so try to GWIM - CHECK GRAMMAR FOR IMPLICIT TAKING???
        if ((required == 1 || required == 2) && frame.Objects.Count == 0 && !TryFindMissing(frame, grammar, true))
        {
            // What do you want to put?
            frame.Error = $"What do you want to {frame.VerbToken}?";
            return false;
        }

        // Indirect object required, but none found, so try to GWIM - CHECK GRAMMAR FOR IMPLICIT TAKING???
        if (required == 2 && frame.IndirectObjects.Count == 0 && !TryFindMissing(frame, grammar, false))
        {
            // What do you want to put the brass lantern on?
            var target = frame.Objects.Count == 1 ? $"the {frame.Objects.First()}" : "those things";
            var prep = frame.Prep ?? grammar.Prepositions.FirstOrDefault();
            var action = $"{frame.VerbToken} {target} {frame.Prep}".Trim();

            // prep might not be set here, so set from grammar
            frame.Prep = prep!;
            frame.Error = $"What do you want to {action} {prep}".Trim() + "?";
            return false;
        }

        return true;
    }

    private static bool TryFindMissing(Frame frame, Grammar grammar, bool direct)
    {
        var objects = GetObjects();
        var xLocByte = direct ? grammar.Object.LocByte : grammar.IndirectObject.LocByte;
        var xGwim = direct ? grammar.Object.Gwim : grammar.IndirectObject.Gwim;
        var target = direct ? frame.Objects : frame.IndirectObjects;

        var matched = GetMatches(xLocByte, objects, xGwim);

        // scenario: in the cellar there are stairs and a chute
        // "climb up" defaults to the chute. Scenery is the
        // only difference I can see.
        var total = matched.Count;
        if (matched.Count > 1)
        {
            var nonScenic = matched.Where(x => !x.Scenery).ToList();
            if (nonScenic.Count > 0)
            {
                matched = nonScenic;
            }
        }

        if (matched.Count == 1)
        {
            // Aside?
            // > turn on
            // (the brass lantern)
            if (total == 1)
            {
                Output.Print($"(the {matched[0]})");
            }

            target.Add(matched[0]);
            return true;
        }

        return false;
    }

    private static List<Object> GetMatches(byte xLocByte, List<Object> objects, Func<Object, bool> filter = null)
    {
        var locByteMap = GetLocBytes(objects);

        List<Object> matched = [];

        foreach (var obj in objects)
        {
            byte locByte = locByteMap[obj];
            bool add = false;

            if (locByte == 0 && (filter != null && filter(obj)))
            {
                add = true;
            }
            else if ((locByte & xLocByte) != 0)
            {
                add = filter == null || filter(obj);
            }

            if (add)
            {
                matched.Add(obj);
            }
        }

        return matched;
    }

    private static List<Object> ResolveByXLocByte(byte xLocByte, List<Object> objects, Func<Object, bool> filter = null)
    {
        var locByteMap = GetLocBytes(objects);

        Dictionary<int, List<Object>> scored = [];

        foreach (var obj in objects)
        {
            byte score = 0b_0000_0000;

            byte locByte = locByteMap[obj];

            if (filter != null && !filter(obj))
            {
                continue;
            }

            if (xLocByte.Has(LocBit.HELD) && locByte.Has(LocBit.HELD))
            {
                score |= 1 << LocBit.HELD;
            }

            if (xLocByte.Has(LocBit.CARRIED) && locByte.Has(LocBit.CARRIED))
            {
                score |= 1 << LocBit.CARRIED;
            }

            if (xLocByte.Has(LocBit.ONGROUND) && locByte.Has(LocBit.ONGROUND))
            {
                score |= 1 << LocBit.ONGROUND;
            }

            if (xLocByte.Has(LocBit.INROOM) && locByte.Has(LocBit.INROOM))
            {
                score |= 1 << LocBit.INROOM;
            }

            if (xLocByte.Has(LocBit.HAVE) && (locByte.Has(LocBit.HELD) || locByte.Has(LocBit.CARRIED)))
            {
                score |= 1 << LocBit.HAVE;
            }

            if (score == 0 && xLocByte == 0)
            {
                score = locByte;
            }

            if (score > 0)
            {
                int closest = Math.Abs(score - xLocByte);

                if (!scored.ContainsKey(closest))
                {
                    scored.Add(closest, []);
                }

                scored[closest].Add(obj);
            }
        }

        if (scored.Count > 0)
        {
            int min = scored.Keys.Min();
            return scored[min];
        }

        return [];
    }

    private static Dictionary<Object, byte> GetLocBytes(Frame frame, Grammar grammar)
    {
        var result = new Dictionary<Object, byte>();
        var xLocByte = grammar.Object.LocByte;

        foreach (var obj in frame.Objects)
        {
            result.TryAdd(obj, GetLocByte(obj, xLocByte));
        }

        if (grammar.IndirectObject != null)
        {
            xLocByte = grammar.IndirectObject.LocByte;

            foreach (var obj in frame.IndirectObjects)
            {
                result.TryAdd(obj, GetLocByte(obj, xLocByte));
            }
        }

        return result;
    }

    private static Dictionary<Object, byte> GetLocBytes(List<Object> objects)
    {
        Dictionary<Object, byte> map = [];

        foreach (var obj in objects)
        {
            map[obj] = GetLocByte(obj);
        }

        return map;
    }

    private static List<Object> GetObjects(bool excludeParent = false)
    {
        var results = SearchList.Top(Player.Instance.Parent);

        if (excludeParent)
        {
            results = [.. results.Where(x => x != Player.Instance.Parent)];
        }

        return results;
    }

    // returns location-related bits only
    public static byte GetLocByte(Object obj, byte? xLocByte = null)
    {
        byte locByte = 0b_0000_0000;

        if (obj.Parent == Player.Instance)
        {
            locByte |= 1 << LocBit.HELD;
        }
        else if (obj.Parent is Container container && container.Open && container.Parent == Player.Instance)
        {
            locByte |= 1 << LocBit.CARRIED;
        }
        else if (obj.Vehicle || obj.Parent == Player.Location || obj.Parent == null) // null here will be scenery objects
        {
            locByte |= 1 << LocBit.ONGROUND;
        }
        else if (obj.Vehicle || obj.Parent?.Parent == Player.Location)
        {
            locByte |= 1 << LocBit.INROOM;
        }

        if (xLocByte != null)
        {
            // copy HAVE, TAKE from grammar
            if (xLocByte.Value.Has(LocBit.HAVE))
            {
                locByte |= 1 << LocBit.HAVE;
            }

            if (xLocByte.Value.Has(LocBit.TAKE))
            {
                locByte |= 1 << LocBit.TAKE;
            }
        }

        return locByte;
    }
}
