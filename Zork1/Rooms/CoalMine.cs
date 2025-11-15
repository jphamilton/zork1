using Zork1.Library;
using Zork1.Library.Utilities;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public abstract class CoalMine : Room
{
    public static PickOne<Room> CoalMineRooms = new([
        Get<Mine4>(),
        Get<Mine3>(),
        Get<Mine2>(),
        Get<Mine1>(),
        Get<LadderTop>(),
        Get<LadderBottom>(),
        Get<SqueakyRoom>(),
        Get<MineEntrance>(),
    ]);

    protected CoalMine()
    {
        Name = "Coal Mine";
        Description = "This is a non-descript part of a coal mine.";
        DryLand = true;
    }
}

public class Mine1 : CoalMine
{
    public Mine1()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        DownTo<LadderTop>();
        WestTo<Mine1>();
        NorthTo<Mine2>();
    }
}

public class Mine2 : CoalMine
{
    public override void Initialize()
    {
        SouthWestTo<Mine1>();
        SouthTo<Mine2>();
        EastTo<Mine3>();
    }
}

public class Mine3 : CoalMine
{
    public override void Initialize()
    {
        SouthEastTo<Mine2>();
        SouthTo<Mine4>();
        NorthTo<Mine3>();
    }
}

public class Mine4 : CoalMine
{
    public override void Initialize()
    {
        NorthEastTo<Mine3>();
        EastTo<Mine4>();
        NorthTo<GasRoom>();
    }
}

public class MineEntrance : Room
{
    public MineEntrance()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Mine Entrance";
        Description = "You are standing at the entrance of what might have been a coal mine. The shaft enters the west wall, " +
            "and there is another exit on the south end of the room.";
        SouthTo<SlideRoom>();
        WestTo<SqueakyRoom>();
    }
}

public class CoalMineDeadEnd : Room
{
    public CoalMineDeadEnd()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Dead End";
        Description = "You have come to a dead end in the mine.";
        IsHere<PileOfCoal>();
        NorthTo<LadderBottom>();
    }
}

public class LadderTop : Room
{
    public LadderTop()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Ladder Top";
        Description = "This is a very small room. In the corner is a rickety wooden ladder, leading downward. " +
            "It might be safe to descend. There is also a staircase leading upward.";
        WithScenery<WoodenLadder, Stairs>();
        UpTo<Mine1>();
        DownTo<LadderBottom>();
    }
}

public class LadderBottom : Room
{
    public LadderBottom()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Ladder Bottom";
        Description = "This is a rather wide room. On one side is the bottom of a narrow wooden ladder. To the west " +
            "and the south are passages leaving the room.";
        WithScenery<WoodenLadder>();
        UpTo<LadderTop>();
        SouthTo<CoalMineDeadEnd>();
        WestTo<TimberRoom>();
    }
}

public class SqueakyRoom : Room
{
    public SqueakyRoom()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Squeaky Room";
        Description = "You are in a small room. Strange squeaky sounds may be heard coming from the passage at the north end. " +
            "You may also escape to the east.";
        NorthTo<BatRoom>();
        EastTo<MineEntrance>();
    }
}
