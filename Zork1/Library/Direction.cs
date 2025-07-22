namespace Zork1.Library;

public abstract class Direction : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        return true;
    }
}
