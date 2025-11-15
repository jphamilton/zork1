using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class DamBase : Room
{
    public DamBase()
    {
        DryLand = true;
        Sacred = true;
        Light = true;
    }

    public override void Initialize()
    {
        Name = "Dam Base";
        Description = "You are at the base of Flood Control Dam #3, which looms above you and to the north. " +
            "The river Frigid is flowing by here. Along the river are the White Cliffs which seem to form " +
            "giant walls stretching from north to south along the shores of the river as it winds its way downstream.";
        WithScenery<Water, River>();
        IsHere<PileOfPlastic>();
        UpTo<Dam>();
        NorthTo<Dam>();
    }
}