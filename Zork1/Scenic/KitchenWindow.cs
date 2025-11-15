using Zork1.Handlers;
using Zork1.Library;
using Zork1.Rooms;

namespace Zork1.Scenic;

public class KitchenWindow : Door
{
    public KitchenWindow()
    {
        Scenery = true;
        Open = false;
    }

    public override void Initialize()
    {
        Name = "kitchen window";
        Adjectives = ["kitchen", "window", "small"];

        DoorDirection(() => Location.Is<Kitchen>() ? Direction<East>() : Direction<West>());
        DoorTo(() => Location.Is<Kitchen>() ? Get<BehindHouse>() : Get<Kitchen>());

        Before<Examine>(() =>
        {
            if (!Visited)
            {
                return Print("The window is slightly ajar, but not enough to allow entry.");
            }

            return false;
        });

        Before<Open, Close>(() =>
        {
            Visited = true;

            return OpenOrClose(
                "With great effort, you open the window far enough to allow entry.",
                "The window closes (more easily than it opened)."
            );
        });

        Before<LookIn>(() =>
        {
            if (Location.Is<Kitchen>())
            {
                return Print("You can see a clear area leading towards a forest.");
            }

            return Print("You can see what appears to be a kitchen.");
        });
    }
}
