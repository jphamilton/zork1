using Zork1.Handlers;
using Zork1.Library;
using Zork1.Rooms;

namespace Zork1.Scenic;

public class TrapDoor : Door
{
    public TrapDoor()
    {
        Scenery = true;
        Concealed = true;
    }

    public override void Initialize()
    {
        Name = "trap door";
        Adjectives = ["trap", "door", "dusty", "trapdoor", "cover"];

        DoorTo(() => Location.Is<LivingRoom>() ? Get<Cellar>() : Get<LivingRoom>());
        DoorDirection(() => Location.Is<LivingRoom>() ? Direction<Down>() : Direction<Up>());

        Before(() =>
        {
            if (Verb is Raise)
            {
                return Redirect.To<Open>(this);
            }

            if (Location is LivingRoom)
            {
                if (Verb is Open || Verb is Close)
                {
                    return OpenOrClose(
                        "The door reluctantly opens to reveal a rickety staircase descending into darkness.",
                        "The door swings shut and closes."
                    );
                }

                if (Verb is LookUnder)
                {
                    if (Open)
                    {
                        return Print("You see a rickety staircase descending into darkness.");
                    }

                    return Print("It's closed.");
                }
            }

            if (Location is not Cellar)
            {
                return false;
            }

            if (Verb is Unlock || Verb is Open && !Open)
            {
                return Print("The door is locked from above.");
            }

            if (Verb is Close && !Open)
            {
                //https://github.com/the-infocom-files/zork1/issues/60
                //Visited = false;
                //Open = false;
                //return Print("The door closes and locks.");
                return Print("The door is already closed.");
            }

            if (Verb is not Handlers.Open && Verb is not Close)
            {
                return false;
            }

            return Print(Tables.Dummy.Pick());
        });
    }
}
