using Zork1.Handlers;

namespace Zork1.Scenic;

public class Songbird : Object
{
    public Songbird()
    {
        Scenery = true;
    }

    public override void Initialize()
    {
        Name = "songbird";
        Adjectives = ["bird", "song", "songbird"];

        Before<Take, Find>(() => Print("The songbird is not here but is probably nearby."));
        Before<Listen>(() => Print("You can't hear the songbird now."));
        Before<Follow>(() => Print("It can't be followed."));

        // handle anything else a player tries to do with the bird
        Before(() => Print("You can't see any songbird here."));

        Daemon = () =>
        {
            if (15 <= Random.Number(1, 100))
            {
                return false;
            }

            return Print("^You hear in the distance the chirping of a song bird.");
        };
    }
}
