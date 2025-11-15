using Zork1.Handlers;
using Zork1.Rooms;
using Zork1.Things;

namespace Zork1.Scenic;

public class Rainbow : Object
{
    public Rainbow()
    {
        Climable = true;
        Scenery = true;
    }

    public override void Initialize()
    {
        Name = "rainbox";
        Adjectives = ["rainbow"];

        Before<Enter, Cross>(() =>
        {
            if (Location.Is<CanyonView>())
            {
                return Print("From here?!?");
            }

            if (Flags.Rainbow)
            {
                if (Location.Is<AragainFalls>())
                {
                    return GoTo<EndOfRainbow>();
                }

                if (Location.Is<EndOfRainbow>())
                {
                    return GoTo<AragainFalls>();
                }

                return Print("You'll have to say which way...");
            }

            return Print("I didn't know you could walk on water vapor.");
        });

        Before<LookUnder>(() => Print("The Frigid River flows under the rainbow."));
    }
}