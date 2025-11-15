using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public abstract class Maze : Room
{
    protected Maze()
    {
        MazeRoom = true;
        DryLand = true;
        Name = "Maze";
        Description = "This is part of a maze of twisty little passages, all alike.";
    }

    protected Room DownMaze()
    {
        Print("You won't be able to get back up to the tunnel you are going through when it gets to the next room.");

        if (Location is Maze4)
        {
            return Get<Maze2>();
        }
        if (Location is Maze7)
        {
            return Get<Maze19>();
        }
        if (Location is Maze15)
        {
            return Get<Maze13>();
        }
        if (Location is Maze12)
        {
            return Get<Maze5>();
        }

        return this;
    }
}

public abstract class DeadEnd : Room
{
    protected DeadEnd()
    {
        MazeRoom = true;
        DryLand = true;
        Name = "Maze";
        Description = "You have come to a dead end in the maze.";
    }
}

public class Maze8 : Maze
{
    public override void Initialize()
    {
        SouthEastTo<CyclopsRoom>();
        SouthTo<Maze7>();
        WestTo<Maze9>();
    }
}

public class Maze9 : Maze
{
    public override void Initialize()
    {
        NorthWestTo<Maze9>();
        NorthEastTo<Maze6>();
        SouthTo<Maze7>();
        WestTo<Maze8>();
    }
}

public class Maze10 : Maze
{
    public override void Initialize()
    {
        DownTo<Maze12>();
        SouthTo<Maze14>();
        WestTo<Maze13>();
        EastTo<Maze15>();
    }
}

public class Maze11 : DeadEnd
{
    public override void Initialize()
    {
        SouthTo<Maze12>();
    }
}

public class Maze12 : Maze
{
    public override void Initialize()
    {
        DownTo(DownMaze);
        UpTo<Maze15>();
        SouthWestTo<Maze13>();
        EastTo<Maze10>();
        NorthTo<Maze11>();
    }
}

public class Maze19 : Maze
{
    public override void Initialize()
    {
        SouthTo<Maze2>();
    }
}

public class Maze2 : Maze
{
    public override void Initialize()
    {
        WestTo<Maze3>();
        EastTo<Maze19>();
        NorthTo<Maze1>();
    }
}

public class Maze3 : Maze
{
    public override void Initialize()
    {
        UpTo<Maze5>();
        WestTo<Maze4>();
        NorthTo<Maze2>();
    }
}

public class Maze4 : Maze
{
    public override void Initialize()
    {
        DownTo(DownMaze);
        SouthTo<Maze1>();
        EastTo<Maze3>();
    }
}

public class Maze1 : Maze
{
    public override void Initialize()
    {
        SouthTo<Maze4>();
        WestTo<Maze2>();
        EastTo<TrollRoom>();
        NorthTo<Maze1>();
    }
}

public class Maze13 : Maze
{
    public override void Initialize()
    {
        DownTo<Maze14>();
        SouthWestTo<Maze12>();
        NorthWestTo<Maze10>();
        NorthEastTo<GratingRoom>();
    }
}

public class Maze14 : Maze
{
    public override void Initialize()
    {
        UpTo<Maze13>();
        WestTo<Maze10>();
        EastTo<Maze15>();
    }
}

public class Maze15 : Maze
{
    public override void Initialize()
    {
        DownTo(DownMaze);
        NorthWestTo<Maze15>();
        SouthTo<Maze10>();
        WestTo<Maze12>();
        EastTo<Maze14>();
        NorthTo<Maze6>();
    }
}

public class Maze16 : DeadEnd
{
    public override void Initialize()
    {
        NorthTo<Maze17>();
    }
}

public class Maze17 : Maze
{
    public override void Initialize()
    {
        SouthEastTo<Maze16>();
        NorthEastTo<Maze7>();
        WestTo<Maze17>();
    }
}

public class Maze7 : Maze
{
    public override void Initialize()
    {
        DownTo(DownMaze);
        UpTo<Maze9>();
        SouthTo<Maze8>();
        WestTo<Maze6>();
        EastTo<Maze17>();
    }
}

public class Maze6 : Maze
{
    public override void Initialize()
    {
        DownTo<Maze5>();
        UpTo<Maze15>();
        WestTo<Maze6>();
        EastTo<Maze7>();
    }
}

public class Maze18 : DeadEnd
{
    public override void Initialize()
    {
        WestTo<Maze5>();
    }
}

public class Maze5 : Maze
{
    public override void Initialize()
    {
        IsHere<Skeleton>();
        IsHere<SkeletonKey>();
        IsHere<RustyKnife>();
        IsHere<BagOfCoins>();
        IsHere<BurnedOutLantern>();

        SouthWestTo<Maze6>();
        EastTo<Maze18>();
        NorthTo<Maze3>();
    }
}