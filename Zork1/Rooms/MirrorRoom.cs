using Zork1.Handlers;
using Zork1.Library;
using Zork1.Things;

namespace Zork1.Rooms;

public abstract class MirrorRoom : Room
{
    protected MirrorRoom()
    {
        Describe = () =>
        {
            var desc = "You are in a large square room with tall ceilings. On the south wall is an enormous mirror " +
            "which fills the entire wall. There are exits on the other three sides of the room.";

            if (Flags.MirrorBroken)
            {
                desc += "^Unfortunately, the mirror has been destroyed by your recklessness.";
            }

            return desc;
        };
    }
}

public class MirrorRoom1 : MirrorRoom
{
    public MirrorRoom1()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Mirror Room";
        WithScenery<Mirror1>();
        WestTo<TwistingPassage>();
        EastTo<Cave2>();
        NorthTo<ColdPassage>();
    }
}

public class MirrorRoom2 : MirrorRoom
{
    public MirrorRoom2()
    {
        DryLand = true;
        Light = true;
    }

    public override void Initialize()
    {
        Name = "Mirror Room";
        Description = "";
        WithScenery<Mirror2>();
        WestTo<WindingPassage>();
        EastTo<Cave1>();
        NorthTo<NarrowPassage>();
    }
}

public abstract class Mirror : Object
{
    protected Mirror()
    {
        TryTake = true;
        Scenery = true;
    }

    public override void Initialize()
    {
        Name = "mirror";
        Adjectives = ["mirror", "reflection", "enormous"];

        Before<Examine, LookIn>(() =>
        {
            if (Flags.MirrorBroken)
            {
                return Print("The mirror is broken into many pieces.");
            }

            return Print("There is an ugly person staring back at you.");
        });

        Before<Take>(() => Print("You would herniate yourself if you did."));

        Before<Touch>(() =>
        {
            if (Flags.MirrorBroken)
            {
                return false;
            }

            if (Second != null && Second is not PairOfHands)
            {
                return Print($"You feel a faint tingling transmitted through the {Second}.");
            }

            MirrorRoom oppositeRoom = Location is MirrorRoom1 ? Get<MirrorRoom2>() : Get<MirrorRoom1>();

            var here = Location.Items;
            var there = oppositeRoom.Items;

            foreach (var obj in here)
            {
                obj.Move(oppositeRoom);
            }

            foreach (var obj in there)
            {
                obj.MoveHere();
            }

            GoTo(oppositeRoom, false);

            return Print("There is a rumble from deep within the earth and the room shakes.");
        });

        Before<Attack, Throw, Poke>(() =>
        {
            if (Flags.MirrorBroken)
            {
                return Print("Haven't you done enough damage already?");
            }

            Flags.MirrorBroken = true;
            Flags.Lucky = false;

            return Print("You have broken the mirror. I hope you have a seven years' supply of good luck handy.");
        });
    }
}

public class Mirror1 : Mirror
{
}

public class Mirror2 : Mirror
{
}