using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class TreasureRoom : Room
{
    public TreasureRoom()
    {
        DryLand = true;
        TakeValue = 25;
    }

    public override void Initialize()
    {
        Name = "Treasure Room";
        Description = "This is a large room, whose east wall is solid granite. A number of discarded bags, " +
            "which crumble at your touch, are scattered about on the floor. There is an exit down a staircase.";
        WithScenery<Stairs>();
        IsHere<Chalice>();
        DownTo<CyclopsRoom>();

        Initial = () =>
        {
            var thief = Get<Thief>();

            thief.StopDaemon();

            if (Flags.Dead)
            {
                return false;
            }

            thief.Fight = true;

            if (!Location.Has(thief))
            {
                Print("^You hear a scream of anguish as you violate the robber's hideaway. Using passages unknown to you, he rushes to its defense.");
                thief.MoveHere();
                thief.Concealed = false;
            }

            ThiefInTreasure();

            return true;
        };
    }

    private void ThiefInTreasure()
    {
        var treasures = Children.Where(x => !x.Concealed && x is not Thief && x is not Chalice && x.Takeable).ToList();

        if (treasures.Count > 0)
        {
            Print("The thief gestures mysteriously, and the treasures in the room suddenly vanish.");
        }

        foreach (var treasure in treasures)
        {
            treasure.Concealed = true;
        }
    }
}
