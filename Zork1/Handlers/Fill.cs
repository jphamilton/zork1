using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Handlers;

public class Fill : Sub
{
    public Fill()
    {
        PreSub = PreFill;
    }

    private bool PreFill(Object noun, Object second)
    {
        var water = Get<Water>();

        if (second == null)
        {
            if (Location.Children.Contains(water))
            {
                // this is very convoluted - WaterAction will be called on the base water object
                return Redirect.To<Insert>(water, noun);
            }

            return Print("There's nothing to fill it with.");
        }

        if (second == water)
        {
            return false;
        }

        // we are filling something else up with something other than water
        return Redirect.To<Insert>(second, noun);
    }

    public override bool Handler(Object noun, Object second)
    {
        if (second == null)
        {
            var (water, quantity_of_water) = Get<Water, QuantityOfWater>();

            // water is scenic so will not have it's parent set, so
            // this type of check is atypical
            if (Location.Children.Contains(water))
            {
                return Redirect.To<Insert>(water, noun);
            }

            if (player.Parent.Has(quantity_of_water))
            {
                return Redirect.To<Insert>(quantity_of_water, noun);
            }

            return Print("There's nothing to fill it with.");
        }

        return Print("You may know how to do that, but I don't.");
    }
}