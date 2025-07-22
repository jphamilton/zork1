using Zork1.Handlers;

namespace Zork1.Scenic;

public class NumberOfGhosts : Object
{
    public NumberOfGhosts()
    {
        Animate = true;
        Scenery = true;
    }

    public override void Initialize()
    {
        Name = "number of ghosts";
        Adjectives = ["ghosts", "spirit", "spirits", "fields", "force"];

        Before<Tell, Hello>(() => Print("The spirits jeer loudly and ignore you."));
        Before<Banish>(() => Print("Only the ceremony itself has any effect."));
        Before<Poke, Attack>(() => Print("How can you attack a spirit with material objects?"));

        Before(() => Print("You seem unable to interact with these spirits."));
    }
}