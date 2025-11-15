using Zork1.Library;

namespace Zork1.Handlers;

public class Close : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (!noun.Container && !noun.Door)
        {
            return Print($"You must tell me how to do that to a {noun}.");
        }

        if ((noun is Container || noun is Door) && !noun.Open)
        {
            return Print("It is already closed.");
        }

        // Changing original code because it seems like you should be able
        // to have containers without setting Capacity, like a small locket
        // that could be opened or closed
        //
        // if (noun is not Supporter && noun.Capacity > 0) - original check
        if (noun is Container)
        {
            noun.Open = false;

            Print("Closed.");

            if (!Lit)
            {
                return true;
            }

            Lit = Query.Light(Location);

            if (Lit)
            {
                return true;
            }

            return Print("It is now pitch black.");
        }

        if (noun is Door)
        {
            noun.Open = false;
            return Print($"The {noun} is now closed.");
        }

        return Print("You cannot close that.");
    }
}