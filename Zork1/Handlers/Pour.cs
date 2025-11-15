using Zork1.Library;
using Zork1.Things;

namespace Zork1.Handlers;

public class Pour : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (noun is QuantityOfWater)
        {
            noun.Remove();

            if (second.Flame && second.Light)
            {
                Print($"The {second} is extinguished.");
                second.Light = false;
                second.Flame = false;
                second.On = false;
                return true;
            }

            return Print($"The water spills over the {second}, to the floor, and evaporates.");
        }

        if (noun is ViscousMaterial)
        {
            return Redirect.To<Insert>(Get<ViscousMaterial>(), second);
        }

        return Print("You can't pour that.");
    }
}