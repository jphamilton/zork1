using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Things;

namespace Zork1.Handlers;

public class North : Direction;

public class Northeast : Direction;

public class Northwest : Direction;

public class East : Direction;

public class West : Direction;

public class South : Direction;

public class Southeast : Direction;

public class Southwest : Direction;

public class Up : Direction;

public class Down : Direction;

public class Enter : Direction
{
    public override bool Handler(Object noun, Object second = null)
    {
        // enter as movement (doors) handled in Command

        if (noun.Vehicle)
        {
            return Redirect.To<Board>(noun);
        }

        if (noun is Me || Player.Has(noun))
        {
            return Print("That would involve quite a contortion!");
        }

        if (!noun.Takeable)
        {
            return Print($"You hit your head against the {noun} as you attempt this feat.");
        }

        return Print(Tables.Yuks.Pick());
    }
}

public class Exit : Direction;

public class Land : Direction;