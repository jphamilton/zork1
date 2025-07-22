using Zork1.Library;

namespace Zork1.Handlers;

public class Say : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        return false;
    }
}

//[ SaySub who ;	! 33794 / 0x8402
//    if (~~P_cont) {
//        "Say what?";
//    }
//    P_quote_flag = false;
//    rtrue;
//    who = FindIn(location,animate);
//    if (who) {
//        print "You must address the ", (name) who, " directly.";
//        new_line;
//        P_quote_flag = false;
//        P_cont = 0;
//        rtrue;
//    }
//    if (P_lexv-->P_cont == 'hello') {
//        P_quote_flag = false;
//        rtrue;
//    }
//    P_quote_flag = false;
//    P_cont = 0;
//    "Talking to yourself is a sign of impending mental collapse.";
//];