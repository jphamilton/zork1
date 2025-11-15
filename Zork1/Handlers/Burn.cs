using Zork1.Library;
using Zork1.Library.Things;

namespace Zork1.Handlers;

public class Burn : Sub
{
    public Burn()
    {
        PreSub = PreBurn;
    }

    private bool PreBurn(Object noun, Object second)
    {
        if (second == null || second.Flame && second.Light)
        {
            return false;
        }

        return Print($"With a {second}??!?");
    }

    public override bool Handler(Object noun, Object second)
    {
        if (noun.Flammable)
        {
            if (Player.Has(noun))
            {
                noun.Remove();
                return JigsUp($"The {noun} catches fire. Unfortunately, you were holding it at the time.");
            }

            noun.Remove();
            return Print($"The {noun} catches fire and is consumed.");
        }

        return Print($"You can't burn a {noun}.");
    }
}