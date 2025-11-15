using Zork1.Library;
using Zork1.Things;

namespace Zork1.Handlers;

public abstract class FineDining : Sub
{
    protected bool EatOrDrink(Object noun, bool drink)
    {
        var canEat = noun.Edible;
        var canDrink = false;

        if (canEat && player.Has(noun))
        {
            if (drink)
            {
                return Print("How can you drink that?");
            }

            noun.Remove();

            return Print("Thank you very much. It really hit the spot.");
        }

        if (noun.Drinkable)
        {
            canDrink = true;

            if (noun is IGlobalObject)
            {
                return QuenchThirst(noun);
            }

            if (!player.Has(noun.Parent))
            {
                return Print($"You have to be holding the {noun.Parent} first.");
            }

            if (!noun.Parent.Open)
            {
                return Print($"You'll have to open the {noun.Parent} first.");
            }

            return QuenchThirst(noun);
        }

        if (canEat || canDrink)
        {
            return false;
        }

        return Print($"I don't think that the {noun} would agree with you.");
    }

    private bool QuenchThirst(Object noun)
    {
        noun.Remove();
        return Print("Thank you very much. I was rather thirsty (from all this talking, probably).");
    }
}
