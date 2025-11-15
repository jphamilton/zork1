using System.Text;
using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class Dam : Room
{
    public Dam()
    {
        DryLand = true;
        Light = true;
    }

    public override void Initialize()
    {
        Name = "Dam";

        Describe = () =>
        {
            StringBuilder sb = new();
            sb.Append("You are standing on the top of the Flood Control Dam #3, which was quite a tourist attraction " +
                "in times far distant. There are paths to the north, south, and west, and a scramble down. ");

            if (Flags.LowTide && Flags.DamOpen)
            {
                sb.Append("You are standing on the top of the Flood Control Dam #3, which was quite a tourist attraction in times far distant. There are paths to the north, south, and west, and a scramble down.");
            }
            else if (Flags.DamOpen)
            {
                sb.Append("The sluice gates are open, and water rushes through the dam. The water level behind the dam is still high.");
            }
            else if (Flags.LowTide)
            {
                sb.Append("The sluice gates are closed. The water level in the reservoir is quite low, but the level is rising quickly.");
            }
            else
            {
                sb.Append("The sluice gates on the dam are closed. Behind the dam, there can be seen a wide reservoir. " +
                    "Water is pouring over the top of the now abandoned dam.");
            }

            sb.Append("^^There is a control panel here, on which a large metal bolt is mounted. Directly above the bolt is a small green plastic bubble");

            if (Flags.Gate)
            {
                sb.Append(" which is glowing serenely");
            }

            sb.Append('.');

            return sb.ToString();
        };

        WithScenery<Water, Bolt, ControlPanel, DamScenery, GreenBubble>();
        DownTo<DamBase>();
        SouthTo<DeepCanyon>();
        WestTo<ReservoirSouth>();
        EastTo<DamBase>();
        NorthTo<DamLobby>();
    }

    public bool RisingWater()
    {
        var (reservoir, deep_canyon, loud_room, trunk_of_jewels) = Get<Reservoir, DeepCanyon, LoudRoom, TrunkOfJewels>();

        reservoir.WaterRoom = true;
        reservoir.DryLand = false;
        deep_canyon.Visited = false;
        loud_room.Visited = false;

        if (reservoir.Has(trunk_of_jewels))
        {
            trunk_of_jewels.Concealed = true;
        }

        Flags.LowTide = false;

        if (Location == reservoir)
        {
            if (player.Parent.Vehicle)
            {
                return Print("The boat lifts gently out of the mud and is now floating on the reservoir.");
            }

            return JigsUp("You are lifted up by the rising river! You try to swim, but the currents are too strong. " +
                "You come closer, closer to the awesome structure of Flood Control Dam #3. The dam beckons to you. " +
                "The roar of the water nearly deafens you, but you remain conscious as you tumble over the dam toward " +
                "your certain doom among the rocks at its base.");
        }

        if (Location == deep_canyon)
        {
            return Print("A sound, like that of flowing water, starts to come from below.");
        }

        if (Location == loud_room)
        {
            Print("All of a sudden, an alarmingly loud roaring sound fills the room. Filled with fear, you scramble away.");
            return GoTo(loud_room.Nearby());
        }

        if (Location.Is<ReservoirNorth>() || Location.Is<ReservoirSouth>())
        {
            return Print("You notice that the water level has risen to the point that it is impossible to cross.");
        }

        return true;
    }

    public bool LowerWater()
    {
        var (reservoir, deep_canyon, loud_room) = Get<Reservoir, DeepCanyon, LoudRoom>();
        var trunk = Get<TrunkOfJewels>();

        reservoir.DryLand = true;
        reservoir.WaterRoom = false;
        deep_canyon.Visited = false;
        loud_room.Visited = false;
        trunk.Concealed = false;
        Flags.LowTide = true;

        if (Location == reservoir && player.Parent.Vehicle)
        {
            return Print("The water level has dropped to the point at which the boat can no longer stay afloat. It sinks into the mud.");
        }

        if (Location == deep_canyon)
        {
            return Print("The roar of rushing water is quieter now.");
        }

        if (Location.Is<ReservoirNorth>() || Location.Is<ReservoirSouth>())
        {
            return Print("You notice that the water level is quite low here, and that you might be able to cross over to the other side.");
        }

        return true;
    }
}
