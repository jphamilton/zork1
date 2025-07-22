using Zork1.Library;

namespace Zork1.Handlers;

public class WalkTo : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (noun.In(Location))
        {
            return Print("It's here!");
        }

        return Print("You should supply a direction!");
    }
}