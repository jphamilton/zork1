using Zork1.Library;

namespace Zork1.Handlers;

public class SwitchOn : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (noun.Switchable)
        {
            if (noun.On)
            {
                return Print("It's already on.");
            }

            noun.On = true;

            return Print($"The {noun} is now on.");
        }

        return Print("You can't turn that on.");
    }
}