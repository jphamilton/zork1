namespace Zork1.Library.Parsing;

public static class SyntaxCheck
{
    public static bool Check(Frame frame, out Grammar grammar)
    {
        grammar = null;

        List<Grammar> verbs = [.. SyntaxDefinitons.Grammars.Where(
            x => x.Verbs.Contains(frame.Verb) || (x.Verbs.Contains(frame.VerbRoot) && x.Prepositions.Contains(frame.Adverb))
        ).Distinct()];

        List<Grammar> preps = frame.Prep != null ? [.. verbs.Where(x => x.Verbs.Contains(frame.Prep) || x.Prepositions.Contains(frame.Prep))] : null;

        List<Grammar> found = preps?.Count > 0 ? preps : verbs.Any(x => x.Prepositions.Count == 0) ? [.. verbs.Where(x => x.Prepositions.Count == 0)] : verbs;

        if (found.Count == 0)
        {
            frame.Error = Messages.DontRecognizeSentence;
            return false;
        }
        else if (found.Count == 1)
        {
            var required = found[0].Required;

            if (required == 0 && frame.Objects.Count > 0)
            {
                // syntax does not require direct objects
                frame.Error = Messages.DontRecognizeSentence;
            }
            else if (required == 1 && frame.IndirectObjects.Count > 0)
            {
                // syntax does not require indirect objects
                frame.Error = Messages.DontRecognizeSentence;
            }
            else if (required == 2 && frame.Objects.Count == 0 && frame.IndirectObjects.Count == 0)
            {
                frame.Error = $"What do you want to {frame.VerbToken}?";
            }
        }
        else if (found.Count > 1)
        {
            var all = found.ToList();

            // narrow routines if more than 1 found

            // 0 = sub does not require objects, 1 = requires direct, 2 = requires direct and indirect
            var required = ((frame.Objects.Count > 0 || frame.UnresolvedObjects.Count > 0) ? 1 : 0) +
                ((frame.IndirectObjects.Count > 0 || frame.UnresolvedIndirectObjects.Count > 0) ? 1 : 0);

            List<Grammar> narrowed = [.. found.Where(g => g.Required == required)];

            if (narrowed.Count > 0)
            {
                found = narrowed;
            }

            // possible incomplete command (e.g. "put" -> "What do you want to put?")
            if (found.Count > 1)
            {
                //-----------------------------------

                // use location bytes to attempt to narrow one last time
                // simple, but pretty sketchy
                List<Grammar> possible = [];

                foreach (var g in found.OrderByDescending(x => x.Object.Gwim != null))
                {
                    var xLocByte = g.Object;
                    var gwim = g.Object.Gwim;

                    foreach (var obj in frame.Objects)
                    {
                        if (gwim != null && !gwim(obj))
                        {
                            continue;
                        }

                        var locByte = Snarf.GetLocByte(obj);
                        //if ((locByte & xLocByte.LocByte) != 0 || (locByte == 0 && xLocByte.LocByte == 0))
                        if ((locByte & xLocByte.LocByte) != 0 || (xLocByte.LocByte == 0))
                        {
                            possible.Add(g);
                            break;
                        }
                    }
                }



                //-----------------------------------
                if (possible.Count == 1)
                {
                    found = possible;
                }
                else if (frame.Prep != null || frame.Adverb != null)
                {
                    // we have something like "put down", "turn on", etc.
                    // find entry with the least amount of required clauses and hope
                    // that SnarfObjects will supply the missing objects
                    found = [.. found.Where(x => x.Required == (found.Min(y => y.Required)))];
                    found = [found[0]];
                }
                else
                {
                    string action = null;

                    if (frame.Objects.Count > 0)
                    {
                        var stuff = frame.Objects.Count == 1 ? $"the {frame.Objects.First()}" : "those things";

                        var g = all[^1];
                        var prep = frame.Prep ?? g.Prepositions.FirstOrDefault();
                        action = $"{frame.VerbToken} {stuff} {prep}".Trim();
                        frame.Required = g.Required;
                        frame.Prep = prep!;
                        frame.Error = $"What do you want to {action}?";
                        return false;
                    }

                    action = $"{frame.VerbToken} {frame.Prep}".Trim();
                    frame.Required = found.Min(x => x.Required);
                    frame.Error = $"What do you want to {action}?";
                    return false;
                }
            }
        }

        grammar = found[0];

        // weird, but we will assign the handler like this
        frame.Sub = grammar.Handler;
        frame.Required = grammar.Required;

        return !frame.IsError;
    }
}
