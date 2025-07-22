using Zork1.Library;

namespace Zork1.Handlers;

public class ThrowOver : Sub
{
    public ThrowOver()
    {
        PreSub = Throw.PreThrow;
    }

    public override bool Handler(Object noun, Object second)
    {
        return Print("You can't throw anything off of that!");
    }
}
