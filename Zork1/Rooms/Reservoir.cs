using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class ReservoirNorth : Room
{
    public ReservoirNorth()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Reservoir North";
        Describe = () =>
        {
            string desc = "";

            if (Flags.LowTide && Flags.DamOpen)
            {
                desc += "You are in a large cavernous room, the south of which was formerly a lake. " +
                "However, with the water level lowered, there is merely a wide stream running through there.";
            }
            else if (Flags.DamOpen)
            {
                desc += "You are in a large cavernous area. To the south is a wide lake, whose water level appears to be falling rapidly.";
            }
            else if (Flags.LowTide)
            {
                desc += "You are in a cavernous area, to the south of which is a very wide stream. The level of the stream is rising rapidly, " +
                "and it appears that before long it will be impossible to cross to the other side.";
            }
            else
            {
                desc += "You are in a large cavernous room, north of a large lake.";
            }

            desc += "^There is a slimy stairway leaving the room to the north.";

            return desc;
        };

        WithScenery<Water, Stairs, PseudoLake>();
        IsHere<AirPump>();
        NorthTo<AtlantisRoom>();
        SouthTo(() => Flags.LowTide ? Get<Reservoir>() : NoGo("You would drown."));
    }
}

public class Reservoir : Room
{
    public Reservoir()
    {
        WaterRoom = true;
    }

    public override void Initialize()
    {
        Name = "Reservoir";
        WithScenery<Water, PseudoStream>();
        IsHere<TrunkOfJewels>();

        Describe = () =>
        {
            if (Flags.LowTide)
            {
                return "You are on what used to be a large lake, but which is now a large mud pile. There are ~shores~ to the north and south.";
            }

            return "You are on the lake. Beaches can be seen north and south. Upstream a small stream enters the lake " +
            "through a narrow cleft in the rocks. The dam can be seen downstream.";
        };

        Before(() =>
        {
            if (!Player.Parent.Vehicle && !Flags.DamOpen && Flags.LowTide)
            {
                return Print("You notice that the water level here is rising rapidly. The currents are also becoming stronger. Staying here seems quite perilous!");
            }

            return false;
        });

        DownTo(() =>
        {
            Print("The dam blocks your way.");
            return this;
        });

        UpTo<InStream>();
        SouthTo<ReservoirSouth>();
        WestTo<InStream>();
        NorthTo<ReservoirNorth>();
    }
}

public class ReservoirSouth : Room
{
    public ReservoirSouth()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Reservoir South";
        WithScenery<Water, PseudoLake, PseudoChasm>();
        Describe = () =>
        {
            string desc;

            if (Flags.LowTide && Flags.DamOpen)
            {
                desc = "You are in a long room, to the north of which was formerly a lake. " +
                "However, with the water level lowered, there is merely a wide stream running through the center of the room.";
            }
            else if (Flags.DamOpen)
            {
                desc = "You are in a long room. To the north is a large lake, too deep to cross. You notice, however, that the " +
                "water level appears to be dropping at a rapid rate. Before long, it might be possible to cross to the other side from here.";
            }
            else if (Flags.LowTide)
            {
                desc = "You are in a long room, to the north of which is a wide area which was formerly a reservoir, but now is merely a stream. " +
                "You notice, however, that the level of the stream is rising quickly and that before long it will be impossible to cross here.";
            }
            else
            {
                desc = "You are in a long room on the south shore of a large lake, far too deep and wide for crossing.";
            }

            return desc + "^^There is a path along the stream to the east or west, a steep pathway climbing southwest along " +
            "the edge of a chasm, and a path leading into a canyon to the southeast.";
        };

        SouthWestTo<Chasm>();
        SouthEastTo<DeepCanyon>();
        WestTo<StreamView>();
        EastTo<Dam>();

        NorthTo(() =>
        {
            if (Flags.LowTide)
            {
                return Get<Reservoir>();
            }

            Print("You would drown.");
            return this;
        });
    }
}
