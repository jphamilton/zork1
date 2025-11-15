using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class Cave1 : Room
{
    public Cave1()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Cave";
        Description = "This is a tiny cave with entrances west and north, and a dark, forbidding staircase leading down.";
        WithScenery<Stairs>();
        DownTo<EntranceToHades>();
        WestTo<WindingPassage>();
        NorthTo<MirrorRoom2>();

        Before(() =>
        {
            var candles = Get<PairOfCandles>();

            if (!Player.Has(candles) || !candles.Light || Random.Probability(50))
            {
                return false;
            }

            candles.StopDaemon();
            candles.Light = false;

            Print("A gust of wind blows out your candles!");

            Lit = Query.Light();

            if (Lit)
            {
                return false;
            }

            return Print("It is now completely dark.");
        });
    }
}

public class Cave2 : Room
{
    public Cave2()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Cave";
        Description = "This is a tiny cave with entrances west and north, and a staircase leading down.";
        WithScenery<Stairs>();
        DownTo<AtlantisRoom>();
        SouthTo<AtlantisRoom>();
        WestTo<TwistingPassage>();
        NorthTo<MirrorRoom1>();
    }
}
