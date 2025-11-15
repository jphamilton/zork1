using Zork1.Handlers;
using Zork1.Things;

namespace Zork1.Scenic;

public class PseudoChasm : Object
{
    private const string ForOnce = "For a change, you look before leaping. You realize you would never survive.";

    public override void Initialize()
    {
        Name = "chasm";
        Adjectives = ["chasm"];

        Before<Dive>(() => Print(ForOnce));

        Before<Insert, ThrowOver>(() =>
        {
            if (Noun is Me)
            {
                return Print(ForOnce);
            }

            if (Second == this)
            {
                Noun.Remove();
                return Print($"The {Noun} drops out of sight into the chasm.");
            }

            return false;
        });

        Before<Cross>(() => Print("It's too far to jump, and there's no bridge."));
    }
}