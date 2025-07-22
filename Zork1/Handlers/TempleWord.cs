using Zork1.Library;
using Zork1.Rooms;

namespace Zork1.Handlers;

public class TempleWord : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (Location is Temple)
        {
            return GoTo<TreasureRoom>();
        }

        if (Location is TreasureRoom)
        {
            return GoTo<Temple>();
        }

        return Print("Nothing happens.");
    }
}
