using System.Text;
using Zork1.Handlers;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class AragainFalls : AboveGround
{
    public override void Initialize()
    {
        Name = "Aragain Falls";
        WithScenery<Water, River, Rainbow>();
        Describe = () =>
        {
            var msg = new StringBuilder("You are at the top of Aragain Falls, an enormous waterfall with a drop of about 450 feet. The only path here is on the north end. ");

            if (Flags.Rainbow)
            {
                msg.Append("A solid rainbow spans the falls.");
            }
            else
            {
                msg.Append("A beautiful rainbow can be seen over the falls and to the west.");
            }

            return msg.ToString();
        };

        NorthTo<Shore>();

        Before<Down>(() => Print("It's a long way..."));
        Before<Up, West>(() =>
        {
            if (Flags.Rainbow)
            {
                return GoTo<OnTheRainbow>();
            }

            return false;
        });
    }
}
