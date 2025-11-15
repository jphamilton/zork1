using Zork1.Library;
using Zork1.Library.Parsing;
using Zork1.Library.Things;
using Zork1.Things;

namespace Zork1.Handlers;

public class Take : Sub
{
    public Take()
    {
        PreSub = PreTake;
    }

    private bool PreTake(Object noun, Object second)
    {
        if (Player.Has(noun))
        {
            if (noun.Clothing && noun.Worn)
            {
                return Print("You are already wearing it.");
            }

            return Print("You already have that!");
        }

        if (noun.Parent is Container container && !container.Open)
        {
            return Print("You can't reach something that's inside a closed container.");
        }

        if (second != null)
        {
            if (second is Ground)
            {
                return false;
            }

            if (second != noun.Parent)
            {
                return Print($"The {noun} isn't in the {second}.");
            }

            return false;
        }

        if (noun == Player.Parent)
        {
            return Print("You're inside of it!");
        }

        return false;
    }

    public override bool Handler(Object noun, Object second)
    {
        if (!TakeCheck.ITake(noun, true))
        {
            return false;
        }

        return Print("Taken.");
    }
}
