using Zork1.Handlers;
using Zork1.Library.Functions;
using Zork1.Rooms;

namespace Zork1.Scenic;

public class Skeleton : Object
{
    public Skeleton()
    {
        TryTake = true;
        Scenery = true;
    }

    public override void Initialize()
    {
        Name = "skeleton";
        Adjectives = ["bones", "skeleton", "body"];
        Before(() =>
        {
            if (Verb is not Handlers.Move && Verb is not Touch && Verb is not Take && Verb is not Lower && Verb is not Raise
                && Verb is not Push && Verb is not Kiss && Verb is not Kick && Verb is not Attack)
            {
                return false;
            }

            Print("A ghost appears in the room and is appalled at your having desecrated the remains " +
                "of a fellow adventurer. He casts a curse on your valuables and banishes them to the " +
                "Land of the Living Dead. The ghost leaves, muttering obscenities.");

            var land_of_the_dead = Get<LandOfTheDead>();

            Rob.Run(Location, land_of_the_dead, 100);
            Rob.Run(player, land_of_the_dead);

            return true;
        });
    }
}
