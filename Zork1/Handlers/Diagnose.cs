using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Melee;

namespace Zork1.Handlers;

public class Diagnose : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        var fightStrength = Player.FightStrength(false);
        var wounds = Player.Strength;
        var remainingStrength = fightStrength + wounds;

        var healFunction = Get<HealFunction>();
        var healTime = Clock.Ticks(healFunction);

        if (healTime == 0)
        {
            wounds = 0;
        }
        else
        {
            wounds = 0 - wounds;
        }

        string diagnosis = null;

        switch (wounds)
        {
            case 0:
                diagnosis = "You are in perfect health.";
                break;
            case 1:
                diagnosis = "You have a light wound,";
                break;
            case 2:
                diagnosis = "You have several wounds,";
                break;
            default:
                diagnosis = "You have serious wounds,";
                break;
        }

        if (wounds > 0)
        {
            diagnosis += $" which will be cured after {healTime} moves.";
        }

        Print(diagnosis);

        if (remainingStrength <= 0)
        {
            Print("You are at death's door.");
        }
        else if (remainingStrength == 1)
        {
            Print("You can be killed by one more light wound.");
        }
        else if (remainingStrength == 3)
        {
            Print("You can survive one serious wound.");
        }
        else
        {
            Print("You are strong enough to take several wounds.");
        }

        if (State.Deaths == 0)
        {
            return true;
        }

        var times = State.Deaths == 1 ? "once" : "twice";

        return Print($"You have been killed {times}.");
    }
}