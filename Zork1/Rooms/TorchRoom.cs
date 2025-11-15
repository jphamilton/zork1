using Zork1.Handlers;
using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class TorchRoom : Room
{
    public TorchRoom()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Torch Room";

        Describe = () =>
        {
            var desc = "This is a large room with a prominent doorway leading to a down staircase. Above you is a large dome. " +
            "Up around the edge of the dome (20 feet up) is a wooden railing. In the center of the room sits a white marble pedestal.";

            if (Flags.Dome)
            {
                desc += "^A piece of rope descends from the railing above, ending some five feet above your head.";
            }

            return desc;
        };

        WithScenery<Stairs>();
        IsHere<Pedestal>();

        DownTo<Temple>();
        SouthTo<Temple>();
        UpTo(() => NoGo("You cannot reach the rope."));

        Before<Kiss>(() => Print("No."));
    }
}
