using Zork1.Handlers;
using Zork1.Library.Things;
using Zork1.Rooms;
using Zork1.Things;

namespace Zork1.Scenic;

public class Bat : Object
{
    public bool Pacified => Flags.Dead || Player.Has<CloveOfGarlic>() || Location.Has<CloveOfGarlic>();

    public Bat()
    {
        TryTake = true;
        Animate = true;
    }

    public override void Initialize()
    {
        Name = "bat";
        Adjectives = ["bat", "vampire", "deranged"];
        Describe = () =>
        {
            if (Pacified)
            {
                return "In the corner of the room on the ceiling is a large vampire bat who is obviously deranged and holding his nose.";
            }
            else
            {
                return "A large vampire bat, hanging from the ceiling, swoops down at you!";
            }
        };

        Before<Tell>(() => Fweep(6));
        Before<Poke, Attack, Take>(() => Print("You can't reach him; he's on the ceiling."));
    }

    private bool Fweep(int count)
    {
        for (var i = 0; i < count; i++)
        {
            Print("    Fweep!");
        }

        return true;
    }

    public bool MovePlayer()
    {
        Fweep(4);
        Print("^The bat grabs you by the scruff of your neck and lifts you away....");
        return GoTo(CoalMine.CoalMineRooms.Pick(), false);
    }
}
