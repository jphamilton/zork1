using Zork1.Library.Parsing;

namespace Zork1.Library;

public abstract class SyntaxBase
{
    protected const byte HAVE = LocBit.HAVE;          // must be in player's possession
    protected const byte MANY = LocBit.MANY;          // multiple objects allowed
    protected const byte TAKE = LocBit.TAKE;          // try to take if necessary
    protected const byte INROOM = LocBit.INROOM;      // not at top-level, contained in another object in the room
    protected const byte ONGROUND = LocBit.ONGROUND;  // at top-level of room and not in container
    protected const byte CARRIED = LocBit.CARRIED;    // not at player level, but inside container - does this even matter?
    protected const byte HELD = LocBit.HELD;          // top level of player

    public abstract SyntaxDefinitons Load();

    public bool Check(Frame frame, out Grammar grammar)
    {
        return SyntaxCheck.Check(frame, out grammar);
    }

    public bool Validate(Grammar grammar, Frame frame)
    {
        return Many.Check(frame, grammar);
    }

    protected ObjSpec OBJECT(Func<Object, bool> gwim = null, params List<int> bits)
    {
        return new ObjSpec
        {
            Gwim = gwim,
            LocByte = Set(bits)
        };
    }

    protected ObjSpec OBJECT(params List<int> bits)
    {
        return new ObjSpec
        {
            LocByte = Set(bits)
        };
    }

    protected ObjSpec OBJECT()
    {
        return new ObjSpec
        {
            LocByte = 0
        };
    }

    protected byte Set(params List<int> bits)
    {
        byte flags = 0;

        foreach (var bit in bits)
        {
            flags |= (byte)(1 << bit);
        }

        return flags;
    }
}
