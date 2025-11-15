using Zork1.Handlers;
using Zork1.Library.Things;

namespace Zork1.Library;

public abstract partial class Object
{
    private readonly Dictionary<Type, Func<bool>> _beforeRoutines = [];
    private readonly Dictionary<Type, Action> _afterRoutines = [];
    private Func<bool> _beforeAll { get; set; }
    private Action _afterAll { get; set; }

    public static Object Noun => Context.Noun;
    public static Object Second => Context.Second;
    public static Sub Verb => Context.Verb;
    public static Room Location => Player.Location;
    protected static Object player => Player.Instance;

    protected static bool Lit
    {
        get
        {
            return State.Lit;
        }
        set
        {
            State.Lit = value;
        }
    }

    // for adding objects to rooms during initialization
    public T IsHere<T>() where T : Object
    {
        Object obj = Objects.Get<T>();
        obj.Remove();
        obj.Parent = this;
        Children.Add(obj);
        return (T)obj;
    }

    // triggers "death"
    public bool JigsUp(string message)
    {
        throw new DeathException(message);
    }

    #region Before Action Helpers

    // generic before routine that captures any command
    // if a Before routine of the specific type was not found
    public void Before(Func<bool> before)
    {
        _beforeAll = before;
    }

    public bool Before<A>() where A : Sub
    {
        return _beforeRoutines.Any(x => x.Key == typeof(A));
    }

    public void Before<A>(Func<string> before) where A : Sub
    {
        bool wrapper()
        {
            var message = before();

            if (message != null)
            {
                return Print(message);
            }

            return false;
        }

        Before<A>(wrapper);
    }

    public void Before<A, B>(Func<bool> before)
        where A : Sub
        where B : Sub
    {
        Before<A>(before);
        Before<B>(before);
    }

    public void Before<A, B, C>(Func<bool> before)
        where A : Sub
        where B : Sub
        where C : Sub
    {
        Before<A>(before);
        Before<B>(before);
        Before<C>(before);
    }

    public void Before<A, B, C, D>(Func<bool> before)
        where A : Sub
        where B : Sub
        where C : Sub
        where D : Sub
    {
        Before<A>(before);
        Before<B>(before);
        Before<C>(before);
        Before<D>(before);
    }

    public void Before<T>(Func<bool> before) where T : Sub
    {
        var type = typeof(T);

        var subTypes = Routines.List?.Where(x => x.GetType().IsSubclassOf(type)).ToList() ?? [];

        _beforeRoutines.TryAdd(type, before);

        foreach (var subType in subTypes)
        {
            _beforeRoutines.TryAdd(subType.GetType(), before);
        }
    }

    public Func<bool> GetBeforeRoutine(Type verbType)
    {
        if (_beforeRoutines.TryGetValue(verbType, out var value))
        {
            return value;
        }

        return _beforeAll;
    }

    public Func<bool> GetBefore<T>()
    {
        if (_beforeRoutines.TryGetValue(typeof(T), out var before))
        {
            return before;
        }

        return null;
    }

    #endregion

    #region After Action Helpers

    public Action GetAfterRoutine(Type verbType)
    {
        if (_afterRoutines.TryGetValue(verbType, out var value))
        {
            return value;
        }

        return _afterAll;
    }

    // generic After routine that captures any command
    // if an After routine of the specific type was not found
    public void After(Action after)
    {
        _afterAll = after;
    }

    public void After<T>(Func<string> after) where T : Sub
    {
        void wrapper()
        {
            var message = after();

            if (message != null)
            {
                Print(message);
            }
        }

        After<T>(wrapper);
    }

    public void After<T>(Action after) where T : Sub => _afterRoutines.Add(typeof(T), after);

    #endregion

    #region Move\\Has\\In

    /// <summary>
    /// Move object to location
    /// </summary>
    /// <param name="to">new location</param>
    public void Move(Object to)
    {
        Remove();
        Parent = to;
        to.Children.Add(this);
    }

    public void Move<T>() where T : Object
    {
        Move(Get<T>());
    }

    /// <summary>
    /// Moves Object to current location
    /// </summary>
    public void MoveHere() => Move(Player.Location);

    public T MoveHere<T>() where T : Object
    {
        var obj = Get<T>();
        obj.MoveHere();
        return obj;
    }

    /// <summary>
    /// Removes an Object from the tree
    /// </summary>
    public void Remove()
    {
        Parent?.Children.Remove(this);
        Parent = null;
    }

    /// <summary>
    /// Object is direct descendant of this
    /// </summary>
    public bool Has<T>() where T : Object
    {
        var obj = Get<T>();
        return Has(obj);
    }

    public bool Has(Object obj)
    {
        return obj.Parent == this;
    }

    /// <summary>
    /// Object is somewhere below this one in the tree
    /// </summary>
    public bool In(Object obj)
    {
        var results = SearchList.All(obj, x => x == this);
        return results.Count == 1;
        //var parent = Parent;

        //while (parent != null)
        //{
        //    if (parent == obj)
        //    {
        //        return true;
        //    }

        //    parent = parent.Parent;
        //}

        //return false;
    }

    #endregion

    #region Movement Helpers

    public bool GoTo<T>(bool showRoomDesc = true) where T : Room
    {
        var room = Get<T>();
        return GoTo(room, showRoomDesc);
    }

    public bool GoTo(Room room, bool showRoomDesc = true)
    {
        var current = Player.Location;

        var before = room.GetBeforeRoutine(typeof(Enter)) ?? room.GetBeforeRoutine(typeof(Go));

        if (before != null && before())
        {
            // handled
            return true;
        }

        var handled = MovePlayer.To(room, showRoomDesc);

        if (handled)
        {
            var after = current.GetAfterRoutine(typeof(Exit));
            after?.Invoke();
        }

        return handled;
    }

    #endregion

    #region Get Object Helpers

    public Object Get(Func<Object, bool> filter)
    {
        return Children.FirstOrDefault(filter);
    }

    public static A Get<A>() where A : Object => Objects.Get<A>();

    public static (A, B) Get<A, B>()
        where A : Object
        where B : Object => Objects.Get<A, B>();

    public static (A, B, C) Get<A, B, C>()
        where A : Object
        where B : Object
        where C : Object => Objects.Get<A, B, C>();

    public static (A, B, C, D) Get<A, B, C, D>()
        where A : Object
        where B : Object
        where C : Object
        where D : Object => Objects.Get<A, B, C, D>();

    public static (A, B, C, D, E) Get<A, B, C, D, E>()
        where A : Object
        where B : Object
        where C : Object
        where D : Object
        where E : Object => Objects.Get<A, B, C, D, E>();

    #endregion

    #region Print Helpers

    protected static bool Print(string message)
    {
        Output.Print(message);
        return true;
    }

    protected static void Debug(string message)
    {
        Output.Debug(message);
    }

    public bool PrintContents(string reveal)
    {
        var last = Items.LastOrDefault();
        if (last != null)
        {
            SetLast.Object(last);
        }

        return Print($"{reveal} {Display.List(Items)}.");
    }

    #endregion
}
