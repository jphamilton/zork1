using Zork1.Handlers;
using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class EntranceToHades : Room
{
    private const string InvisibleForces = "Some invisible force prevents you from passing through the gate.";
    private int TimeLeft { get; set; }

    public EntranceToHades()
    {
        DryLand = true;
        Light = true;
    }

    public override void Initialize()
    {
        Name = "Entrance to Hades";

        Describe = () =>
        {
            var desc = "You are outside a large gateway, on which is inscribed^^  " +
            "Abandon every hope all ye who enter here!^^The gate is open; through it you can see a desolation, " +
            "with a pile of mangled bodies in one corner. Thousands of voices, lamenting some hideous fate, can be heard.";

            if (Flags.LLD || Flags.Dead)
            {
                desc += "^^The way through the gate is barred by evil spirits, who jeer at your attempts to pass.";
            }

            return desc;
        };

        WithScenery<NumberOfGhosts, PileOfBodies, PseudoGate>();

        UpTo<Cave1>();
        InTo(() => Flags.LLD ? Get<LandOfTheDead>() : NoGo(InvisibleForces));
        SouthTo(() => Flags.LLD ? Get<LandOfTheDead>() : NoGo(InvisibleForces));

        var (brass_bell, black_book, pair_of_candles, red_hot_bell) = Get<BrassBell, BlackBook, PairOfCandles, RedHotBell>();

        Before<Banish>(() =>
        {
            if (Flags.LLD)
            {
                return false;
            }

            if (player.Has(brass_bell) && player.Has(black_book) && player.Has(pair_of_candles))
            {
                return Print("You must perform the ceremony.");
            }

            return Print("You aren't equipped for an exorcism.");
        });

        Before<Ring>(() =>
        {
            if (Flags.LLD)
            {
                return false;
            }

            Flags.ExorcismBell = true;
            brass_bell.Remove();
            red_hot_bell.MoveHere();
            SetLast.Object(red_hot_bell);

            Print("The bell suddenly becomes red hot and falls to the ground. The wraiths, as if paralyzed, stop their " +
                "jeering and slowly turn to face you. On their ashen faces, the expression of a long-forgotten terror takes shape.");

            if (player.Has(pair_of_candles))
            {
                Print("^In your confusion, the candles drop to the ground (and they are out).");

                pair_of_candles.MoveHere();
                pair_of_candles.Light = false;
                pair_of_candles.StopDaemon();
            }

            Clock.Queue(red_hot_bell.CoolBell, 20);
            TimeLeft = 6;
            StartDaemon();
            return true;
        });

        Before<Read>(() =>
        {
            if (!Flags.ExorcismCandle || Noun is not BlackBook || Flags.LLD)
            {
                return false;
            }

            var number_of_ghosts = Get<NumberOfGhosts>();
            number_of_ghosts.Remove();

            Flags.LLD = true;
            Flags.ExorcismCandle = false;
            Flags.ExorcismBell = false;

            StopDaemon();

            return Print("Each word of the prayer reverberates through the hall in a deafening confusion. " +
                "As the last word fades, a voice, loud and commanding, speaks: ~Begone, fiends!~ " +
                "A heart-stopping scream fills the cavern, and the spirits, sensing a greater power, flee through the walls.");
        });

        Daemon = () =>
        {
            TimeLeft--;

            if (TimeLeft == 0)
            {
                Flags.ExorcismCandle = false;
                Flags.ExorcismBell = false;
                StopDaemon();

                if (Location == this)
                {
                    return Print("The tension of this ceremony is broken, and the wraiths, amused but shaken at your clumsy attempt, " +
                        "resume their hideous jeering.");
                }
            }

            if (!Flags.ExorcismCandle && player.Has(pair_of_candles) && pair_of_candles.Light)
            {
                Flags.ExorcismCandle = true;
                TimeLeft = 3;
                Print("The flames flicker wildly and appear to dance. The earth beneath your feet trembles, " +
                "and your legs nearly buckle beneath you. The spirits cower at your unearthly power.");
            }

            return true;
        };
    }
}

public static class Exorcism
{
    public static bool Daemon()
    {
        return false;
    }
}