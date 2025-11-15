using Zork1.Handlers;
using Zork1.Library;
using Zork1.Things;

namespace Zork1.Scenic;

public class Carpet : Object
{
    public Carpet()
    {
        Scenery = true;
        TryTake = true;
    }

    public override void Initialize()
    {
        Name = "carpet";
        Adjectives = ["carpet", "rug", "large", "oriental"];

        Before<Raise>(() =>
        {
            if (Flags.RugMoved)
            {
                return Print("The rug is too heavy to lift.");
            }

            return Print("The rug is too heavy to lift, but in trying to take it you have noticed an irregularity beneath it.");
        });

        Before<Push, Move>(() =>
        {
            if (Flags.RugMoved)
            {
                return Print("Having moved the carpet previously, you find it impossible to move it again.");
            }

            Print("With a great effort, the rug is moved to one side of the room. With the rug moved, the dusty cover of a closed trap door appears.");
            var trapdoor = Get<TrapDoor>();
            trapdoor.Concealed = false;
            SetLast.Object(trapdoor);
            Flags.RugMoved = true;
            return true;
        });

        Before<Take>(() => Print("The rug is extremely heavy and cannot be carried."));

        Before<LookUnder>(() =>
        {
            var trapdoor = Get<TrapDoor>();
            if (!Flags.RugMoved && !trapdoor.Open)
            {
                return Print("Underneath the rug is a closed trap door. As you drop the corner of the rug, the trap door is once again concealed from view.");
            }

            return false;
        });

        Before<ClimbOn>(() =>
        {
            var trapdoor = Get<TrapDoor>();

            if (!Flags.RugMoved && !trapdoor.Open)
            {
                return Print("You sit down. The rug seems to have an irregularity underneath it.");
            }

            return Print("I suppose you think it's a magic carpet?");
        });
    }
}
