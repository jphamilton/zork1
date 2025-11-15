using Zork1.Library;
using Zork1.Things;

namespace Zork1.Handlers;

public class Find : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        var env = noun.Parent;

        if (noun is PairOfHands || noun is BlastOfAir)
        {
            return Print("Within six feet of your head, assuming you haven't left that somewhere.");
        }

        if (noun is Me)
        {
            return Print("You're around here somewhere...");
        }

        if (noun is IGlobalObject)
        {
            return Print("You find it.");
        }

        if (player.Has(noun))
        {
            return Print("You have it.");
        }

        if (Location.Has(noun))
        {
            return Print("It's right here.");
        }

        if (env.Animate)
        {
            return Print($"The {env} has it.");
        }

        if (env is Supporter)
        {
            return Print($"It's on the {env}.");
        }

        if (env is Container c && c.CanSeeContents)
        {
            return Print($"It's in the {env}.");
        }

        return Print("Beats me.");
    }
}