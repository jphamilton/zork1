using Zork1.Things;

namespace Zork1.Library.Locations;

public class Darkness : Room
{
    public Darkness()
    {
        Light = false;
        Describe = () =>
        {
            var desc = "It is pitch black.";

            if (!Flags.GrueRepellent)
            {
                desc += " You are likely to be eaten by a grue.";
            }

            return desc;
        };
    }

    public override void Initialize()
    {

    }
}
