using Zork1.Handlers;
using Zork1.Rooms;

namespace Zork1.Things;

public class RedHotBell : Object
{
    public RedHotBell()
    {
        TryTake = true;
    }

    public override void Initialize()
    {
        Name = "red hot brass bell";
        Description = "On the ground is a red hot bell.";
        Adjectives = ["bell", "brass", "hot", "red", "small"];

        Before<Take>(() => Print("The bell is very hot and cannot be taken."));

        //https://github.com/the-infocom-files/zork1/issues/58
        Before<Touch>(() =>
        {
            if (Second == null || Second is PairOfHands)
            {
                return Print("The bell is too hot to touch.");
            }

            return false;
        });

        Before<Ring>(() =>
        {
            if (Second != null)
            {
                if (Second.Flammable)
                {
                    Second.Remove();
                    return Print($"The {Second} burns and is consumed.");
                }

                if (Second is PairOfHands)
                {
                    return Print("The bell is too hot to touch.");
                }

                return Print("The heat from the bell is too intense.");
            }

            return false;
        });

        Before<Pour>(() =>
        {
            // https://github.com/the-infocom-files/zork1/issues/49
            if (Noun is QuantityOfWater)
            {
                Noun.Remove();
                Print("The water cools the bell and is evaporated.");
                return CoolBell();
            }

            return false;
        });
    }

    public bool CoolBell()
    {
        Remove();

        var brass_bell = Get<BrassBell>();

        brass_bell.Move<EntranceToHades>();

        if (Location is EntranceToHades)
        {
            return Print("The bell appears to have cooled down.");
        }

        return true;
    }
}
