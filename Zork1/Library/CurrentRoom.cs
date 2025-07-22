using Zork1.Library.Locations;
using Zork1.Library.Things;

namespace Zork1.Library;

public static class CurrentRoom
{
    public static bool IsLit()
    {
        var lit = Query.Light(Player.Location);
        State.Lit = lit;
        return lit;
    }

    public static void CheckLight(Room originalRoom, bool wasLit)
    {
        // player was in darkness, did not move, and light source was turned on
        if (!wasLit && IsLit() && Player.Location == originalRoom)
        {
            // will move player to actual room and description
            // of room will be displayed
            MovePlayer.To(originalRoom);
            return;
        }

        // player had light, did not move, and light source was turned off
        if (wasLit && Player.Location == originalRoom && !IsLit())
        {
            Output.Print("^It is now pitch black.");
        }
    }

    public static void Look(bool force = false, bool lit = false)
    {
        Output.NewLine();

        bool isLit = lit || IsLit();

        Room room = isLit ? Player.Location : Objects.Get<Darkness>();
       
        if (Player.InVehicle)
        {
            var vehicle = Player.Parent;
            
            if (isLit)
            {
                Output.Bold($"{room.Name} (in a {vehicle})");
            }
        }
        else
        {
            Output.Bold(room.Name);
        }

        if (!force && State.SuperBrief)
        {
            return;
        }

        string desc = null;
        var brief = !State.SuperBrief && !State.Verbose && !Player.Location.Visited;

        if (force || brief || State.Verbose)
        {
            desc = room.Describe != null ? room.Describe() : room.Description;
        }

        if (isLit)
        {
            DescribeObjects(desc);
        }
        else if (desc != null)
        {
            Output.Print(desc);
        }

        room.Described();
    }

    private static void DescribeObjects(string roomDesc)
    {
        var objects = Player.Location.Items;

        List<string> output = [];

        foreach (var obj in objects)
        {
            if (obj is Supporter s && TryDisplaySupporter(s, out string desc))
            {
                output.Add(desc);
                continue;
            }

            if (TrySkipScenery(obj))
            {
                continue;
            }

            DisplayObject(obj, output);
        }

        Output.Print(roomDesc);

        if (output.Count > 0)
        {
            foreach (var line in output)
            {
                Output.NewLine();
                Output.Print(line);
            }
        }
    }

    private static bool TryDisplaySupporter(Supporter supporter, out string desc)
    {
        desc = Describer.Object(supporter);
        return desc != null;
    }

    private static string GetInitialOrDescribe(Object obj)
    {
        if (obj.ShowInitial)
        {
            return obj.Initial;
        }
        else if (obj.Describe != null)
        {
            return obj.Describe();
        }
        else if (obj.Description != null)
        {
            return obj.Description;
        }

        return null;
    }

    private static void DisplayContainer(Container c, List<string> output)
    {
        int described = 0;
        List<string> descriptions = [];

        var initial = GetInitialOrDescribe(c);
        if (initial != null)
        {
            described++;
            descriptions.Add(initial);
        }

        if (!c.CanSeeContents || c.Items.Count == 0)
        {
            if (descriptions.Count > 0)
            {
                output.Add(descriptions[0]);
            }
            else
            {
                // not displaying (which is empty)
                output.Add($"There is a {c} here.");
            }
            
            return;
        }

        var contents = c.Items;

        foreach(var item in contents.Where(x => x.ShowInitial || x.Describe != null).ToList())
        {
            initial = GetInitialOrDescribe(item);
            if (initial != null)
            {
                descriptions.Add(initial);
                contents.Remove(item);
                described++;
            }
        }

        if (described > 0)
        {
            if (contents.Count > 0 && c.Open)
            {
                descriptions.Add($"The {c} also contains {Display.List(contents)}.");
            }

            var combined = string.Concat(descriptions);
            var sentences = combined.Split('.').Where(x => !string.IsNullOrEmpty(x)).ToList();
            combined =  string.Join(". ", sentences.Select(x => x.Trim()));
            output.Add($"{combined}.");
        }
        else
        {
            var desc = c.Open ? contents.Count > 0 ? $"a {c} (which contains {Display.List(contents)})" : $"a {c} (which is empty)" : $"a {c}";
            output.Add($"There is {desc} here.");
        }
    }

    private static void DisplayObject(Object obj, List<string> output)
    {
        if (Player.Parent.Vehicle && obj == Player.Parent)
        {
            var boatObjects = obj.Items.Where(x => x != Player.Instance).ToList();
            if (boatObjects.Count > 0)
            {
                output.Add($"Inside the {obj} is {Display.List(boatObjects)}.");
            }
        }
        else if (obj is Container c)
        {
            DisplayContainer(c, output);
        }
        else if (!DisplayInitialOrDescribe(obj, output))
        {
            output.Add($"There is a {obj} here.");
        }
    }

    private static bool DisplayInitialOrDescribe(Object obj, List<string> output)
    {
        if (obj.ShowInitial)
        {
            output.Add(obj.Initial);
            return true;
        }
        else if (obj.Describe != null)
        {
            var describe = obj.Describe();

            if (describe != null)
            {
                output.Add(describe);
                return true;
            }
        }
        else if (obj.Description != null)
        {
            output.Add(obj.Description);
            return true;
        }
        
        return false;
    }

    private static bool TrySkipScenery(Object obj)
    {
        if (obj.Scenery && obj.Describe == null)
        {
            return true;
        }

        return false;
    }
}
