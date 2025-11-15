using Zork1.Handlers;
using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Rooms;
using Zork1.Scenic;

namespace Zork1.Things;

public class Rope : Object
{
    public Rope()
    {
        Size = 10;
        Sacred = true;
        TryTake = true;
        Takeable = true;
    }

    public override void Initialize()
    {
        Name = "rope";
        Adjectives = ["rope", "hemp", "coil"];
        Initial = "A large coil of rope is lying in the corner.";

        Before<TieTo>(() =>
        {
            if (Location is not DomeRoom)
            {
                Flags.Dome = false;
                // https://github.com/the-infocom-files/zork1/issues/10
                //return Print("You can't tie the rope to that.");
                return false;
            }

            if (Second is not WoodenRailing)
            {
                return false;
            }

            if (Flags.Dome)
            {
                return Print("The rope is already tied to it.");
            }

            Print("The rope drops over the side and comes within ten feet of the floor.");

            Flags.Dome = true;
            Scenery = true;

            Move(Location);

            return true;
        });

        Before<ClimbDown>(() =>
        {
            if (Flags.Dome && Noun == this)
            {
                return Redirect.To<Down>();
            }

            return false;
        });

        Before<TieWith>(() =>
        {
            if (Noun is Villain villain && Second == this)
            {
                if (villain.Strength < 0)
                {
                    Print($"Your attempt to tie up the {Noun} awakens him.");
                    return villain.CheckStrength();
                }

                return Print($"The {Noun} struggles and you cannot tie him up.");
            }

            return Print($"Why would you tie up a {Noun}?");
        });

        Before<Untie>(() =>
        {
            if (Flags.Dome)
            {
                Flags.Dome = false;
                Scenery = false;
                return Print("The rope is now untied.");
            }

            return Print("It is not tied to anything.");
        });

        Before<Drop>(() =>
        {
            if (!Flags.Dome && Location.Is<DomeRoom>())
            {
                Move<TorchRoom>();
                return Print("The rope drops gently to the floor below.");
            }

            return false;
        });

        Before<Take>(() =>
        {
            if (Flags.Dome)
            {
                return Print("The rope is tied to the railing.");
            }

            return false;
        });
    }
}