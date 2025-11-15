using Zork1.Handlers;
using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class Studio : Room
{
    public Studio()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Studio";
        Description = "This appears to have been an artist's studio. The walls and floors are splattered with paints of 69 different colors. Strangely enough, nothing of value is hanging here. At the south end of the room is an open door (also covered with paint). A dark and narrow chimney leads up from a fireplace; although you might be able to get up it, it seems unlikely you could get back down.";
        WithScenery<Chimney>();
        IsHere<ZorkOwnersManual>();

        UpTo(() =>
        {
            if (Inventory.Count == 0)
            {
                Print("Going up empty-handed is a bad idea.");
                return this;
            }

            if (Inventory.Count <= 2 && player.Has<BrassLantern>())
            {
                var trapdoor = Get<TrapDoor>();
                if (!trapdoor.Open)
                {
                    trapdoor.Visited = false;
                    return Get<Kitchen>();
                }

                return Get<Kitchen>();
            }

            Print("You can't get up there with what you're carrying.");

            return this;
        });

        SouthTo<Gallery>();
    }
}
