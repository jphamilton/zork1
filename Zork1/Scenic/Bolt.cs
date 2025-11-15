using Zork1.Handlers;
using Zork1.Library;
using Zork1.Rooms;
using Zork1.Things;

namespace Zork1.Scenic;

public class Bolt : Object
{
    public Bolt()
    {
        Scenery = true;
        TryTake = true;
        Turnable = true;
    }

    public override void Initialize()
    {
        Name = "bolt";
        Adjectives = ["bolt", "nut", "metal", "large"];
        Before<Take>(() => Print(ControlPanel.Integral));
        Before<Grease>(() => Print("Hmm. It appears the tube contained glue, not oil. Turning the bolt won't get any easier...."));
        Before<MoveWith>(() =>
        {
            var (dam, reservoir_south, loud_room) = Get<Dam, ReservoirSouth, LoudRoom>();

            if (Second is not Wrench)
            {
                return Print($"The bolt won't turn using the {Second}.");
            }

            if (!Flags.Gate)
            {
                return Print("The bolt won't turn with your best effort.");
            }

            reservoir_south.Visited = false;

            if (Flags.DamOpen)
            {
                Flags.DamOpen = false;

                loud_room.Visited = false;
                Print("The sluice gates close and water starts to collect behind the dam.");

                Clock.Queue(dam.RisingWater, 8);
                Clock.Queue(dam.LowerWater, 0);

                return true;
            }

            Flags.DamOpen = true;

            Print("The sluice gates open and water pours through the dam.");

            Clock.Queue(dam.LowerWater, 8);
            Clock.Queue(dam.RisingWater, 0);

            return true;
        });
    }
}
