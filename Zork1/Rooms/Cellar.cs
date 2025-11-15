using Zork1.Handlers;
using Zork1.Library;
using Zork1.Scenic;

namespace Zork1.Rooms;

public class Cellar : Room
{
    public Cellar()
    {
        TakeValue = 25;
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Cellar";
        WithScenery<TrapDoor, Chute, Stairs>();

        Describe = () => "You are in a dark and damp cellar with a narrow passageway leading north, and a crawlway to the south. On the west is the bottom of a steep metal ramp which is unclimbable.";

        NorthTo<TrollRoom>();

        WestTo(() =>
        {
            Print("You try to ascend the ramp, but it is impossible, and you slide back down.");
            return this;
        });

        SouthTo<EastOfChasm>();

        Before<Enter>(() =>
        {
            var trapDoor = Get<TrapDoor>();

            if (!trapDoor.Open || trapDoor.Visited)
            {
                return false;
            }

            trapDoor.Open = false;
            trapDoor.Visited = true;

            Print("The trap door crashes shut, and you hear someone barring it.");
            return false;
        });
    }
}
