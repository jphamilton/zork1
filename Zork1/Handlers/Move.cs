using Zork1.Library;

namespace Zork1.Handlers;

public class Move : Sub
{
    public Move()
    {
        PreSub = PreMove;
    }

    public bool PreMove(Object noun, Object second)
    {
        if (!noun.In(player))
        {
            return false;
        }

        return Print("You aren't an accomplished enough juggler.");
    }

    public override bool Handler(Object noun, Object second)
    {
        if (noun.Takeable)
        {
            return Print($"Moving the {noun} reveals nothing.");
        }

        return Print($"You can't move the {noun}.");
    }
}