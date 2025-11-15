using Zork1.Library;
using Zork1.Things;

namespace Zork1.Rooms;

public class Gallery : Room
{
    public Gallery()
    {
        DryLand = true;
        Light = true;
    }

    public override void Initialize()
    {
        Name = "Gallery";
        Description = "This is an art gallery. Most of the paintings have been stolen by vandals with exceptional taste. The vandals left through either the north or west exits.";
        IsHere<Painting>();
        WestTo<EastOfChasm>();
        NorthTo<Studio>();
    }
}
