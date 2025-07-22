using Zork1.Library;
using Zork1.Library.Things;

namespace Zork1.Handlers;

public class Wake : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (noun is Villain villain)
        {
            if (villain.Strength < 0)
            {
                Print($"The {noun} is rudely awakened.");
                return villain.CheckStrength();
            }

            return Print("He's wide awake, or haven't you noticed...");
        }

        return Print($"The {noun} isn't sleeping.");
    }
}