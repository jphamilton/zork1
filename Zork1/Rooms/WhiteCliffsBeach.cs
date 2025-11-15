using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public abstract class BeachRoom : Room
{
    public const string TooNarrow = "The path is too narrow.";

    protected BeachRoom()
    {
        DryLand = true;
        Sacred = true;
    }

    public override void Initialize()
    {

        Before(() =>
        {
            var boat = Get<MagicBoat>();

            if (Player.Has(boat))
            {
                boat.Deflated = false;
            }

            boat.Deflated = true;

            return false;
        });
    }
}

public class Beach1 : BeachRoom
{
    public override void Initialize()
    {
        base.Initialize();
        Name = "White Cliffs Beach";
        Description = "You are on a rocky, narrow strip of beach beside the Cliffs. A narrow path leads north along the shore.";
        WithScenery<Water, WhiteCliffs, River>();

        var boat = Get<MagicBoat>();

        NorthTo(() => boat.Deflated ? Get<Beach2>() : NoGo(TooNarrow));
    }
}

public class Beach2 : BeachRoom
{
    public override void Initialize()
    {
        base.Initialize();
        Name = "White Cliffs Beach";
        Description = "You are on a narrow strip of beach which runs along the base of the White Cliffs. There is a narrow path heading south along the Cliffs and a tight passage leading west into the cliffs themselves.";
        WithScenery<Water, WhiteCliffs, River>();

        var boat = Get<MagicBoat>();

        SouthTo(() => boat.Deflated ? Get<Beach1>() : NoGo(TooNarrow));
        WestTo(() => boat.Deflated ? Get<DampCave>() : NoGo(TooNarrow));
    }
}