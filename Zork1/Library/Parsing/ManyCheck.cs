using Zork1.Library.Extensions;

namespace Zork1.Library.Parsing;

public static class Many
{

    public static bool Check(Frame frame, Grammar grammar)
    {
        Func<string, string> TooMany = (x) => $"You can't use multiple {x} objects with \"{frame.Verb}\".";

        // TODO: need errors when objects were supplied but not needed????

        if (grammar.Required == 0)
        {
            return true;
        }

        if (grammar.Required > 0)
        {
            // check direct objects
            var xLocByte = grammar.Object.LocByte;

            if (frame.Objects.Count > 1 && !xLocByte.Has(LocBit.MANY))
            {
                frame.Error = TooMany("direct");
                return false;
            }
        }

        if (grammar.Required > 1)
        {
            var xLocByte = grammar.IndirectObject.LocByte;

            if (frame.IndirectObjects.Count > 1 && !xLocByte.Has(LocBit.MANY))
            {
                frame.Error = TooMany("indirect");
                return false;
            }
        }

        return true;
    }
}
