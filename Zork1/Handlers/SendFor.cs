using Zork1.Library;

namespace Zork1.Handlers;

public class SendFor : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (noun.Animate)
        {
            return Print($"Why would you send for the {noun}?");
        }

        return Print("That doesn't make sends.");
    }
}