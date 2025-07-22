using Zork1.Library;
using Zork1.Library.Things;

namespace Zork1.Handlers;

public class Disembark : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (player.Parent != noun)
        {
            return Print("You're not in that!");
        }

        if (Location.DryLand)
        {
            Print("You are on your own feet again.");
            player.Move(Location);
            return true;
        }

        return Print("You realize that getting out here would be fatal.");
    }
}