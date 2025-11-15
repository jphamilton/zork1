using Zork1.Library;

namespace Zork1.Handlers;

public class SwitchOff : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (noun.Switchable)
        {
            if (!noun.On)
            {
                return Print("It's already off.");
            }

            noun.On = false;

            return Print($"The {noun} is now off.");
        }

        return Print("You can't turn that off.");
    }
}
