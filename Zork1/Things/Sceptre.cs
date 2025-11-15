using Zork1.Handlers;
using Zork1.Rooms;

namespace Zork1.Things;

public class Sceptre : Object
{
    public Sceptre()
    {
        TrophyValue = 6;
        TakeValue = 4;
        Takeable = true;
        Weapon = true;
        Size = 3;
    }

    public override void Initialize()
    {
        Name = "sceptre";
        Adjectives = ["sceptre", "scepter", "egyptian", "ancient", "enamel", "treasure"];
        Description = "An ornamented sceptre, tapering to a sharp point, is here.";
        Initial = "A sceptre, possibly that of ancient Egypt itself, is in the coffin. The sceptre is ornamented with colored enamel, and tapers to a sharp point.";

        Before<Raise, Wield>(() =>
        {
            var (on_the_rainbow, pot_of_gold) = Get<OnTheRainbow, PotOfGold>();

            if (Location is AragainFalls || Location is EndOfRainbow)
            {
                // adding concealed check
                // https://github.com/the-infocom-files/zork1/issues/32
                if (!Flags.Rainbow && pot_of_gold.Concealed)
                {
                    pot_of_gold.Concealed = false;
                    Print("Suddenly, the rainbow appears to become solid and, I venture, walkable (I think the giveaway was the stairs and bannister).");

                    if (Location is EndOfRainbow)
                    {
                        Print("^A shimmering pot of gold appears at the end of the rainbow.");
                    }

                    Flags.Rainbow = true;

                    return true;
                }

                // if we have stuff sitting on the rainbow...it's gone now
                on_the_rainbow.Items.ForEach(obj => obj.Remove());

                Flags.Rainbow = false;

                return Print("The rainbow seems to have become somewhat run-of-the-mill.");
            }

            if (Location == on_the_rainbow)
            {
                Flags.Rainbow = false;

                return JigsUp("The structural integrity of the rainbow is severely compromised, " +
                    "leaving you hanging in mid-air, supported only by water vapor. Bye.");
            }

            return Print("A dazzling display of color briefly emanates from the sceptre.");
        });
    }
}