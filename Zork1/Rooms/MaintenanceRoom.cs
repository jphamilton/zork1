using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class MaintenanceRoom : Room
{
    public int LeakSprung { get; set; }

    public MaintenanceRoom()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Name = "Maintenance Room";
        Description = "This is what appears to have been the maintenance room for Flood Control Dam #3. " +
            "Apparently, this room has been ransacked recently, for most of the valuable equipment is gone. " +
            "On the wall in front of you is a group of buttons colored blue, yellow, brown, and red. " +
            "There are doorways to the west and south.";
        WithScenery<Leak, BlueButton, RedButton, BrownButton, YellowButton>();

        IsHere<ToolChests>();
        IsHere<Wrench>();
        IsHere<Tube>();
        IsHere<Screwdriver>();

        SouthTo<DamLobby>();
        WestTo<DamLobby>();
    }

    public bool TriggerLeak()
    {
        var leak = Get<Leak>();

        leak.Concealed = false;
        leak.StartDaemon();

        LeakSprung = 1;

        return Print("There is a rumbling sound and a stream of water appears to burst from the east wall " +
            "of the room (apparently, a leak has occurred in a pipe).");
    }
}