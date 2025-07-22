using Zork1.Library;
using Zork1.Library.Things;

namespace Zork1.Handlers;
public class Drop : Sub
{
    public Drop()
    {
        PreSub = PreDrop;
    }

//[ PreDropSub ;	! 30360 / 0x7698
//    if (noun ~= parent(player)) rfalse;
//    Perform(##Disembark,noun);	! not popped
//    rtrue;
//];
    public bool PreDrop(Object noun, Object second)
    {
        if (noun != player.Parent)
        {
            return false;
        }

        return Redirect.To<Disembark>(noun);
    }

    public override bool Handler(Object noun, Object second)
    {
        if (!PerformDrop(noun, Location))
        {
            return false;
        }

        return Print("Dropped.");
    }

    // called from other places
    public static bool PerformDrop(Object noun, Room location)
    {
        if (!Player.Has(noun) && !Player.Has(noun.Parent))
        {
            Print($"You're not carrying the {noun}.");
            return false;
        }

        if (!Player.Has(noun) && !noun.Parent.Open)
        {
            Print($"The {noun} is closed.");
            return false;
        }

        noun.Move(Player.Instance.Parent);
        return true;
    }
}
