using Zork1.Library;
using Zork1.Library.Parsing;

namespace Zork1.Handlers;

public class Wear : Sub
{
    public override bool Handler(Object noun, Object _)
    {
        if (!noun.Clothing)
        {
            return Print($"You can't wear the {noun}.");
        }

        if (noun.Worn)
        {
            return Print("You are already wearing it.");
        }

        if (!TakeCheck.ITake(noun, true))
        {
            return false;
        }

        noun.Worn = true;

        return true;
    }
}