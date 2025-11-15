using Zork1.Handlers;
using Zork1.Library;
using Zork1.Library.Things;

namespace Zork1.Things;

public class Sword : Object
{
    public int VillainNear { get; set; }

    private const string Bright = "^Your sword is glowing very brightly.";
    private const string FaintGlow = "^Your sword is glowing with a faint blue glow.";

    public Sword()
    {
        Size = 30;
        Takeable = true;
        TrophyValue = 0;
        TryTake = true;
        Weapon = true;
    }

    public override void Initialize()
    {
        Name = "sword";
        Adjectives = ["sword", "elvish", "old", "antique", "orcarist", "glamdring", "blade"];
        Initial = "Above the trophy case hangs an elvish sword of great antiquity.";

        Before<Examine>(() =>
        {
            if (VillainNear == 1)
            {
                return Print(FaintGlow);
            }

            if (VillainNear != 2)
            {
                return Print(Bright);
            }

            return false;
        });

        Daemon = SwordDaemon;
    }

    private bool SwordDaemon()
    {
        if (!Player.Has(this))
        {
            return true;
        }

        if (Infested(Player.Location))
        {
            if (VillainNear == 2)
            {
                return true;
            }

            VillainNear = 2;
            Print(Bright);
            return true;
        }
        else if (InfestedNextDoor())
        {
            if (VillainNear == 1)
            {
                return true;
            }

            VillainNear = 1;
            Print(FaintGlow);
            return true;
        }

        if (VillainNear > 0)
        {
            VillainNear = 0;
            Print("^Your sword is no longer glowing.");
        }

        return true;
    }

    private bool Infested(Room room) => room.Children.Any(o => o is Villain);

    private bool InfestedNextDoor()
    {
        foreach (var room in Location.AdjoiningRooms)
        {
            if (room is Door door && Infested(door.DoorTo()))
            {
                return true;
            }
            else if (Infested(room))
            {
                return true;
            }
        }

        return false;
    }
}
