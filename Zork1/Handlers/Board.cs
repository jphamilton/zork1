using Zork1.Library;

namespace Zork1.Handlers;

public class Board : Sub
{
    public Board()
    {
        PreSub = PreBoard;
    }

    private bool PreBoard(Object noun, Object second)
    {
        var env = player.Parent;

        if (noun.Vehicle)
        {
            if (!Location.Has(noun))
            {
                return Print($"The {noun} must be on the ground to be boarded.");
            }

            if (!env.Vehicle)
            {
                return false;
            }

            return Print($"You are already in the {noun}!");
        }

        return Print($"You have a theory on how to board a {noun}, perhaps?");
    }

    public override bool Handler(Object noun, Object second)
    {
        Print($"You are now in the {noun}.");
        // not normal room-to-room movement, player parent is now vehicle
        player.Move(noun);
        return true;
    }
}