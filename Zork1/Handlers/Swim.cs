using Zork1.Library;
using Zork1.Scenic;

namespace Zork1.Handlers;

public class Swim : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        var water = Get<Water>();
        if (Location.Has(water))
        {
            var loc = noun != null ? $"{noun}" : "dungeon";
            return Print($"Swimming isn't usually allowed in the {loc}.");
        }

        return Print("Go jump in a lake!");
    }
}