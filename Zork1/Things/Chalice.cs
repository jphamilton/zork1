using Zork1.Handlers;
using Zork1.Library;
using Zork1.Rooms;

namespace Zork1.Things;

public class Chalice : Container
{
    public Chalice()
    {
        Capacity = 5;
        Size = 10;
        TrophyValue = 5;
        TakeValue = 10;
        TryTake = true;
        Takeable = true;
    }

    public override void Initialize()
    {
        Name = "chalice";
        Adjectives = ["chalice", "cup", "silver", "treasure", "engravings"];
        Description = "There is a silver chalice, intricately engraved, here.";
        Before<LookIn, Close, Open>(() => Print("You can't do that."));
        Before<Examine>(() => Print($"It looks pretty much like a {Name}."));
        Before<Insert>(() => Second == this && Print("You can't. I guess the chalice wasn't intended to be used that way."));

        Before<Take>(() =>
        {
            var (thief, treasure_room) = Get<Thief, TreasureRoom>();

            if (!treasure_room.Has(this) || !treasure_room.Has(thief) || !thief.Fight || thief.Concealed || thief.Description == Thief.ThiefOutDesc)
            {
                return false;
            }

            return Print("Realizing just in time that you'd be stabbed in the back if you attempted to take the chalice, you return to the fray.");
        });
    }
}