using Zork1.Library;

namespace Zork1.Handlers;

public class Tell : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (noun.Animate)
        {
            //if (P_cont)
            //{
            //    player = noun;
            //    location = parent(player);
            //    return location;
            //}
            return Print($"The {noun} pauses for a moment, perhaps thinking that you should re-read the manual.");
        }

        Print($"You can't talk to the {noun}!");
        //P_quote_flag = false;
        //P_cont = 0;
        //return 2;
        return true;
    }
}