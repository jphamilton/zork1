using Zork1.Handlers;
using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class GasRoom : Room
{
    public GasRoom()
    {
        DryLand = true;
        Sacred = true;
    }

    public override void Initialize()
    {
        Name = "Gas Room";
        Description = "This is a small room which smells strongly of coal gas. There is a short climb up some " +
            "stairs and a narrow tunnel leading east.";
        WithScenery<PseudoGas, Stairs>();
        IsHere<SapphireEncrustedBraclet>();
        UpTo<SmellyRoom>();
        EastTo<Mine4>();

        Before(() =>
        {
            var (pair_of_candles, torch, matchbook) = Get<PairOfCandles, Torch, Matchbook>();
            bool burn_flag = false;

            if (Verb is Burn || Verb is SwitchOn && (Noun == pair_of_candles || Noun == matchbook || Noun == torch))
            {
                burn_flag = true;
            }

            if ((!pair_of_candles.In(player) || !pair_of_candles.Light) &&
                (!torch.In(player) || !torch.Light) &&
                (!matchbook.In(player) || !matchbook.Light))
            {
                return false;
            }

            if (burn_flag)
            {
                Print($"How sad for an aspiring adventurer to light a {Noun} in a room which reeks of gas. Fortunately, there is justice in the world.");
            }
            else
            {
                Print("Oh dear. It appears that the smell coming from this room was coal gas. I would have thought twice about carrying flaming objects in here.");
            }

            return JigsUp("^      ** BOOOOOOOOOOOM **");
        });
    }
}
