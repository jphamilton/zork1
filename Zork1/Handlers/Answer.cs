using Zork1.Library;

namespace Zork1.Handlers;

public class Answer : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        return Print("Nobody seems to be awaiting your answer.");
    }
}