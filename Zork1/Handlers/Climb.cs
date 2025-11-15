using Zork1.Library;

namespace Zork1.Handlers;

public class Climb : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (Location.CanGo<Up>())
        {
            return GoUpOrDown<Up>(noun);
        }
        else if (Location.CanGo<Down>())
        {
            return GoUpOrDown<Down>(noun);
        }

        return Print("You can't go that way.");
    }

    protected bool GoUpOrDown<T>(Object noun) where T : Direction, new()
    {
        if (noun?.Climable == false)
        {
            return Print("You can't do that!");
        }

        if (!Location.CanGo<T>())
        {
            var dir = typeof(T) == typeof(Up) ? "upward" : "downward";

            if (noun != null)
            {
                var x = noun.PluralName ? "don't" : "doesn't";
                return Print($"The {noun} {x} lead {dir}.");
            }

            return Print("You can't go that way.");
        }

        var room = Location.RoomTo<T>();

        if (room != null)
        {
            return GoTo(room);
        }

        return false;
    }
}

public class ClimbUp : Climb
{
    public override bool Handler(Object noun, Object second)
    {
        return GoUpOrDown<Up>(noun);
    }
}

public class ClimbDown : Climb
{
    public override bool Handler(Object noun, Object second)
    {
        return GoUpOrDown<Down>(noun);
    }
}

public class ClimbOn : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        // this is modified from the original - because I haven't figured out the intent
        //if (noun has vehicle) {
        //    return ClimbUp(u_to, pair_of_hands);
        //}
        if (noun.Vehicle)
        {
            return Redirect.To<Board>(noun);
        }

        return Print($"You can't climb onto the {noun}.");
    }
}