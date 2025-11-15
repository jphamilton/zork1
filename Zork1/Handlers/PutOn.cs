using Zork1.Library;
using Zork1.Things;

namespace Zork1.Handlers;

public class PutOn : Sub
{
    public PutOn()
    {
        PreSub = (noun, _) => Give.PreGiveTo(noun, null);
    }

    public override bool Handler(Object noun, Object second)
    {
        if (second is Ground)
        {
            return Redirect.To<Drop>(noun);
        }

        if (second is Supporter)
        {
            return Redirect.To<Insert>(noun, second);
        }

        return Print($"There's no good surface on the {second}.");
    }
}