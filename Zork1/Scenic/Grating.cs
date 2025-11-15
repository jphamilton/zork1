using Zork1.Handlers;
using Zork1.Library;
using Zork1.Rooms;
using Zork1.Things;

namespace Zork1.Scenic;

public class Grating : Door
{
    public Grating()
    {
        Concealed = true;
        Scenery = true;
        Open = false;
        Locked = true;
    }
    public override void Initialize()
    {
        Name = "grating";
        Adjectives = ["grate", "grating"];

        var gratingRoom = Get<GratingRoom>();
        var leaves = Get<PileOfLeaves>();

        Before<Lock>(() =>
        {
            if (Location == gratingRoom)
            {
                // https://github.com/the-infocom-files/zork1/issues/71
                if (Open)
                {
                    Open = false;
                    Print("(first closing the grate)");
                }

                if (Locked)
                {
                    return Print("The grate is already locked.");
                }

                Locked = true;
                return Print("The grate is locked.");
            }

            if (Location is Clearing1)
            {
                return Print("You can't lock it from this side.");
            }

            return false;
        });

        Before<Unlock>(() =>
        {
            if (Location == gratingRoom && Second is SkeletonKey)
            {
                if (!Locked)
                {
                    return Print("The grate is already unlocked.");
                }

                Locked = false;
                return Print("The grate is unlocked.");
            }

            if (Location is Clearing1 && Second is SkeletonKey)
            {
                return Print("You can't reach the lock from here.");
            }

            return Print($"Can you unlock a grating with a {Second}?");
        });

        Before<Pick>(() => Print($"You can't pick the lock."));

        Before<Open, Close>(() =>
        {
            if (Verb is Open && Second is SkeletonKey key)
            {
                return Redirect.To<Unlock>(this, key);
            }

            if (!Locked)
            {
                OpenOrClose(Location is Clearing1 ? "The grating opens." : "The grating opens to reveal trees above you.", "The grating is closed.");

                if (Open)
                {
                    if (Location == gratingRoom && !leaves.Moved)
                    {
                        Print("A pile of leaves falls onto your head and to the ground.");
                        leaves.Moved = true;
                        leaves.Move(Location);
                    }

                    gratingRoom.Light = true;
                    return true;
                }

                gratingRoom.Light = false;
                return true;
            }

            return Print("The grating is locked.");
        });

        Before<Insert>(() =>
        {
            if (Second != this)
            {
                return false;
            }

            if (Noun.Size > 20)
            {
                return Print("It won't fit through the grating.");
            }

            // https://github.com/the-infocom-files/zork1/issues/51
            if (Location is GratingRoom)
            {
                Noun.Move<Clearing1>();
                return Print($"The {Noun} goes through the grating into the light above.");
            }
            else
            {
                Noun.Move<GratingRoom>();
                return Print($"The {Noun} goes through the grating into the darkness below.");
            }
        });
    }
}
