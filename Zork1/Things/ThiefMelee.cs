using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Rooms;

namespace Zork1.Things;

public partial class Thief : Villain
{
    public override bool OnDisarmed()
    {
        if (Has(stiletto) || !Parent.Has(stiletto))
        {
            return false;
        }

        stiletto.Move(this);
        stiletto.Scenery = true;

        if (!Location.Has(this))
        {
            return true;
        }

        return Print("The robber, somewhat surprised at this turn of events, nimbly retrieves his stiletto.");
    }

    public override bool OnKilled()
    {
        stiletto.MoveHere();
        stiletto.Scenery = false;

        var treasures = ThiefDies(Location);

        if (Location is TreasureRoom)
        {
            if (Location.Has<Chalice>())
            {
                Print("^The chalice is now safe to take.");
            }

            if (treasures.Count > 0)
            {
                foreach (var obj in treasures)
                {
                    obj.Concealed = false;
                }

                Print("^As the thief dies, the power of his magic decreases, and his treasures reappear:");
                Print(Describer.DisplayList(treasures));
            }

        }

        if (treasures.Count > 0)
        {
            Print("^His booty remains.");
        }

        StopDaemon();

        return true;
    }

    public override bool OnAttacked()
    {
        if (!Location.Has<Thief>() || Concealed || 20 <= Random.Number(100))
        {
            return false;
        }

        Fight = true;

        return true;
    }

    public override bool OnKnockedOut()
    {
        var stiletto = MoveHere<Stiletto>();
        stiletto.Scenery = false;
        Fight = false;
        Description = ThiefOutDesc;
        StopDaemon();
        return true;
    }

    public override bool OnWakeUp()
    {
        if (Parent == Location)
        {
            Fight = true;
            Print("The robber revives, briefly feigning continued unconsciousness, and, when he sees his moment, scrambles away from you.");
        }

        StartDaemon();
        Description = ThiefDesc;

        return RetreiveStiletto();
    }
}
