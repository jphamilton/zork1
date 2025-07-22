using Zork1.Library.Things;

namespace Zork1.Library;

public abstract class DefaultSub(string Message) : Sub
{
    public override bool Handler(Object noun, Object second) => Print(Message);
}

public abstract class ShortSub(Func<Object, Object, bool> ActionHandler) : Sub
{
    public override bool Handler(Object noun, Object second) => ActionHandler(noun, second);
}

public abstract class Sub
{
    protected Object player => Player.Instance;
    public Room Location => Player.Location;

    public abstract bool Handler(Object noun, Object second);

    public bool Handler()
    {
        return Handler(null, null);
    }

    // If true, move counter is not incremented (e.g. score, i)
    public bool NoTurn { get; set; }

    /// <summary>
    /// Pre-validates conditions for Sub (optional) and allows
    /// command execution to halt early on.
    /// </summary>
    public Func<Object, Object, bool> PreSub { get; set; } = null;

    public static T Get<T>() where T : Object => Objects.Get<T>();

    public static (A, B) Get<A, B>() where A : Object where B : Object => (Objects.Get<A>(), Objects.Get<B>());

    public static (A, B, C) Get<A, B, C>()
        where A : Object
        where B : Object
        where C : Object => (Objects.Get<A>(), Objects.Get<B>(), Objects.Get<C>());

    public static (A, B, C, D) Get<A, B, C, D>()
        where A : Object
        where B : Object
        where C : Object
        where D : Object => (Objects.Get<A>(), Objects.Get<B>(), Objects.Get<C>(), Objects.Get<D>());

    public static (A, B, C, D, E) Get<A, B, C, D, E>()
        where A : Object
        where B : Object
        where C : Object
        where D : Object
        where E : Object => (Objects.Get<A>(), Objects.Get<B>(), Objects.Get<C>(), Objects.Get<D>(), Objects.Get<E>());

    public bool GoTo<T>(bool showRoomDesc = true) where T : Room
    {
        return MovePlayer.To<T>(showRoomDesc);
    }

    public bool GoTo(Room room, bool showRoomDesc = true)
    {
        return MovePlayer.To(room, showRoomDesc);
    }

    public static bool Lit
    {
        get { return State.Lit; }
        set { State.Lit = value; }
    }

    protected static bool Print(string message)
    {
        Output.Print(message);
        return true;
    }

    protected bool JigsUp(string message)
    {
        throw new DeathException(message);
    }
}
