using Zork1.Library;
using Zork1.Rooms;

namespace Zork1.Handlers;

public class Pray : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (Location is Altar)
        {
            return GoTo<Forest1>();
        }

        return Print("If you pray enough, your prayers may be answered.");
    }
};
