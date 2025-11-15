using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class DeepCanyon : Room
{
    public DeepCanyon()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Deep Canyon";
        WithScenery<Stairs>();
        Describe = () =>
        {
            var desc = "You are on the south edge of a deep canyon. Passages lead off to the east, northwest and southwest. A stairway leads down.";

            if (Flags.DamOpen && !Flags.LowTide)
            {
                desc += " You can hear a loud roaring sound, like that of rushing water, from below.";
            }
            else
            {
                if (!Flags.DamOpen && Flags.LowTide)
                {
                    return desc;
                }

                desc += " You can hear the sound of flowing water from below.";
            }

            return desc;
        };

        DownTo<LoudRoom>();
        NorthWestTo<ReservoirSouth>();
        EastTo<Dam>();
        SouthWestTo<NorthSouthPassage>();
    }
}