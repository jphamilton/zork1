using Zork1.Handlers;
using Zork1.Library.Things;
using Zork1.Rooms;
using Zork1.Things;

namespace Zork1.Scenic;

public class Leak : Object
{
    public MaintenanceRoom maintenance_room => Get<MaintenanceRoom>();

    public bool InDanger { get; set; }

    private List<string> LeakLevel = new([
        "up to your ankles.",
        "up to your shin.",
        "up to your knees.",
        "up to your hips.",
        "up to your waist.",
        "up to your chest.",
        "up to your neck.",
        "over your head.",
        "high in your lungs.",
    ]);

    public Leak()
    {
        Concealed = true;
        Scenery = true;
    }

    public override void Initialize()
    {
        Name = "leak";
        Adjectives = ["leak", "drip", "pipe"];

        Daemon = () =>
        {
            InDanger = Location is MaintenanceRoom;

            if (InDanger)
            {
                Print($"^The water level here is now {LeakLevel[maintenance_room.LeakSprung / 2]}");
            }

            maintenance_room.LeakSprung++;

            // https://github.com/the-infocom-files/zork1/issues/36
            if (maintenance_room.LeakSprung >= 16)
            {
                maintenance_room.RMUNGBIT = true;
                maintenance_room.Description = "The room is full of water and cannot be entered.";
                
                if (!InDanger)
                {
                    return true;
                }

                return JigsUp("I'm afraid you have done drowned yourself.");
            }

            if (Player.InVehicle)
            {
                return JigsUp("The rising water lifts the boat and carries it through the door, past the lobby, and over the falls. Tsk, tsk.");
            }

            return true;
        };

        Before<PutOn, Insert>(() =>
        {
            if (maintenance_room.LeakSprung <= 0)
            {
                return false;
            }

            if (Noun is ViscousMaterial)
            {
                return FixLeak();
            }

            return false;
        });

        Before<Fix>(() =>
        {
            if (maintenance_room.LeakSprung <= 0)
            {
                return false;
            }

            if (Second is ViscousMaterial)
            {
                return FixLeak();
            }

            return Print($"With a {Second}?");
        });
    }

    private bool FixLeak()
    {
        maintenance_room.LeakSprung = 65535;
        StopDaemon();
        return Print("By some miracle of Zorkian technology, you have managed to stop the leak in the dam.");
    }
}
