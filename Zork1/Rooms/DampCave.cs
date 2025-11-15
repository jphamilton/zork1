using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class DampCave : Room
{
    public DampCave()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Damp Cave";
        Description = "This cave has exits to the west and east, and narrows to a crack toward the south. The earth is particularly damp here.";
        WithScenery<Crack>();
        SouthTo(() => NoGo("It is too narrow for most insects."));
        WestTo<LoudRoom>();

        // https://github.com/the-infocom-files/zork1/issues/31
        var boat = Get<MagicBoat>();
        EastTo(() => boat.Deflated ? Get<Beach2>() : NoGo(BeachRoom.TooNarrow));
    }
}