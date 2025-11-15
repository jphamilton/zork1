using Zork1.Handlers;
using Zork1.Library;
using Zork1.Library.Extensions;
using Zork1.Rooms;
using Zork1.Things;

namespace Zork1.Scenic;

public class River : Object
{
    private List<Room> RiverNext = [];
    private Dictionary<Room, int> RiverSpeed = [];

    public River()
    {
        Scenery = true;
    }

    public override void Initialize()
    {
        Name = "river";
        Adjectives = ["river", "frigid"];

        var (river4, river3, river2, frigid_river, river1) = Get<River4, River3, River2, FrigidRiver, River1>();
        var mine1 = Get<Mine1>();

        RiverNext = [
            river4,
            river3,
            river2,
            frigid_river,
            river1
        ];

        RiverSpeed = new Dictionary<Room, int> {
            { river4, 4},
            { river3, 4},
            { river2, 3},
            { frigid_river, 2},
            { river1, 1},
        };

        Before<Insert>(() =>
        {
            if (Second != this)
            {
                return false;
            }

            if (Noun is Me)
            {
                return JigsUp("You splash around for a while, fighting the current, then you drown.");
            }

            if (Noun is MagicBoat)
            {
                return Print("You should get in the boat then launch it.");
            }

            if (Noun.Flammable)
            {
                return Print($"The {Noun} floats for a moment, then sinks.");
            }

            Noun.Remove();

            return Print($"The {Noun} splashes into the water and is gone forever.");
        });

        Before<Enter, Dive>(() => Print("A look before leaping reveals that the river is wide and dangerous, " +
            "with swift currents and large, sharp, half-hidden rocks. You therefore decide to forgo your ill-considered swim."));
    }

    public void Start(Room river)
    {
        var speed = RiverSpeed[river];
        Clock.Queue(RiverDaemon, speed);
    }

    public bool RiverDaemon()
    {
        if (!Location.Is<River4, River3, River2, River1, FrigidRiver>())
        {
            Clock.Interrupt(RiverDaemon);
            return true;
        }

        var room = RiverNext.GetNext(false);

        if (room != null)
        {
            Print("The flow of the river carries you downstream.");
            GoTo(room);
            var speed = RiverSpeed[Location];
            Clock.Queue(RiverDaemon, speed);
            return true;
        }

        return JigsUp("Unfortunately, the magic boat doesn't provide protection from unfriendly rocks " +
            "and boulders one meets at the bottom of many waterfalls. Including this one.");
    }
}