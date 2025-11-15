using Zork1.Handlers;
using Zork1.Things;

namespace Zork1.Scenic;

public class Cliff : Wall
{
    public Cliff()
    {
        Climable = true;
        Scenery = true;
    }

    public override void Initialize()
    {
        Name = "cliff";
        Adjectives = ["rocky", "sheer", "wall", "cliff", "walls", "ledge"];

        const string fatal = "That would be very unwise. Perhaps even fatal.";

        Before<Dive>(() => Print(fatal));

        Before<ThrowOver, Insert>(() =>
        {
            if (Noun is Me)
            {
                return Print(fatal);
            }

            if (Second is not Cliff)
            {
                return false;
            }

            Print($"The {Noun} tumbles end over end into the river and is seen no more.");
            Noun.Remove();
            return true;
        });
    }
}