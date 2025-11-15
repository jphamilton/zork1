using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class SandyBeach : Room
{
    public SandyBeach()
    {
        DryLand = true;
        Sacred = true;
    }

    public override void Initialize()
    {
        Name = "Sandy Beach";
        Description = "You are on a large sandy beach on the east shore of the river, which is flowing quickly by. A path runs beside the river to the south here, and a passage is partially buried in sand to the northeast.";
        WithScenery<Water, River>();
        NorthEastTo<SandyCave>();
        SouthTo<Shore>();
        IsHere<Shovel>();
    }
}
