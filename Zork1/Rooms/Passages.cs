using Zork1.Library;

namespace Zork1.Rooms;

public class TwistingPassage : Room
{
    public TwistingPassage()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Twisting Passage";
        Description = "This is a winding passage. It seems that there are only exits on the east and north.";
        NorthTo<MirrorRoom1>();
        EastTo<Cave2>();
    }
}

public class WindingPassage : Room
{
    public WindingPassage()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Winding Passage";
        Description = "This is a winding passage. It seems that there are only exits on the east and north.";
        EastTo<Cave1>();
        NorthTo<MirrorRoom1>();
    }
}

public class NarrowPassage : Room
{
    public NarrowPassage()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Narrow Passage";
        Description = "This is a long and narrow corridor where a long north-south passageway briefly narrows even further.";
        SouthTo<MirrorRoom2>();
        NorthTo<RoundRoom>();
    }
}

public class ColdPassage : Room
{
    public ColdPassage()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Cold Passage";
        Description = "This is a cold and damp corridor where a long east-west passageway turns into a southward path.";
        SouthTo<MirrorRoom1>();
        WestTo<SlideRoom>();
    }
}