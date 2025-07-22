using Zork1.Library;

namespace Zork1.Handlers;

public class MoveWith : Sub
{
    public MoveWith()
    {
        PreSub = PreMoveWith;

    }

    public bool PreMoveWith(Object noun, Object second)
    {
        if (noun.Turnable)
        {
            return false;
        }

        return Print("You can't turn that!");
    }

    public override bool Handler(Object noun, Object second)
    {
        return Print("This has no effect.");
    }
}
