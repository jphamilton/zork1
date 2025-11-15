using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Things;

namespace Zork1.Handlers;

public class ThrowTo : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        // teeth has synonym "overboard" - funny implementation detail
        if (Player.Has(noun) && second is SetOfTeeth)
        {
            // https://github.com/the-infocom-files/zork1/issues/50
            var env = Player.Parent;

            if (env.Vehicle)
            {
                // added check for buoy which should float, but everything
                // else should sink and disappear forever
                if (env.Parent.WaterRoom && noun is not RedBuoy)
                {
                    noun.Remove();
                }
                else
                {
                    noun.Move(env.Parent);
                }

                return Print($"Ahoy -- {noun} overboard!");
            }

            return Print("You're not in anything!");
        }

        return Print("Huh?");
    }
}