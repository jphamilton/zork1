using System.Diagnostics;
using System.Text.Json.Serialization;
using Zork1.Handlers;
using Zork1.Library.Things;

namespace Zork1.Library;

[DebuggerDisplay("{Name,nq}")]
public abstract class Room : Object
{
    private readonly Dictionary<Type, Func<Room>> _roomMap = [];
    private readonly Dictionary<Type, Room> _adjoiningRooms = [];

    public bool MazeRoom { get; set; }

    public Room RoomTo<T>() where T : Direction
    {
        if (CanGo<T>())
        {
            var roomer = _roomMap[typeof(T)];
            return roomer();
        }

        return null;
    }

    public bool CanGo<T>() where T : Direction
    {
        return _roomMap.ContainsKey(typeof(T));
    }

    protected Room NoGo(string message)
    {
        Output.Print(message);
        return this;
    }

    [JsonIgnore]
    // called the first time a player enters the room
    public new Func<bool> Initial { get; set; }

    [JsonIgnore]
    // called after the room has been described, M_WAKE/M-FLASH room arg
    public Action Described { get; set; } = () => { };

    #region Room Map

    private void AddToRoomMap<T, D>()
        where T : Room
        where D : Direction
    {
        Room room = Get<T>();

        if (room != null)
        {
            _roomMap.Remove(typeof(D));
            _roomMap.Add(typeof(D), () => room);

            _adjoiningRooms.Add(typeof(D), room);
        }
    }

    private void AddToRoomMap<D>(Func<Room> getRoom) where D : Direction
    {
        _roomMap.Remove(typeof(D));
        _roomMap.Add(typeof(D), getRoom);
    }

    public void LandTo<T>() where T : Room
    {
        AddToRoomMap<T, Land>();
    }

    public void LandTo(Func<Room> getRoom)
    {
        AddToRoomMap<Land>(getRoom);
    }

    public void NorthTo<T>() where T : Room
    {
        AddToRoomMap<T, North>();
    }

    public void NorthTo(Func<Room> getRoom)
    {
        AddToRoomMap<North>(getRoom);
    }

    public void SouthTo<T>() where T : Room
    {
        AddToRoomMap<T, South>();
    }

    public void SouthTo(Func<Room> getRoom)
    {
        AddToRoomMap<South>(getRoom);
    }

    public void EastTo<T>() where T : Room
    {
        AddToRoomMap<T, East>();
    }

    public void EastTo(Func<Room> getRoom)
    {
        AddToRoomMap<East>(getRoom);
    }

    public void WestTo<T>() where T : Room
    {
        AddToRoomMap<T, West>();
    }

    public void WestTo(Func<Room> getRoom)
    {
        AddToRoomMap<West>(getRoom);
    }

    public void NorthWestTo<T>() where T : Room
    {
        AddToRoomMap<T, Northwest>();
    }

    public void NorthWestTo(Func<Room> getRoom)
    {
        AddToRoomMap<Northwest>(getRoom);
    }

    public void NorthEastTo<T>() where T : Room
    {
        AddToRoomMap<T, Northeast>();
    }

    public void NorthEastTo(Func<Room> getRoom)
    {
        AddToRoomMap<Northeast>(getRoom);
    }

    public void SouthWestTo<T>() where T : Room
    {
        AddToRoomMap<T, Southwest>();
    }

    public void SouthWestTo(Func<Room> getRoom)
    {
        AddToRoomMap<Southwest>(getRoom);
    }

    public void SouthEastTo<T>() where T : Room
    {
        AddToRoomMap<T, Southeast>();
    }

    public void SouthEastTo(Func<Room> getRoom)
    {
        AddToRoomMap<Southeast>(getRoom);
    }

    public void InTo<T>() where T : Room
    {
        AddToRoomMap<T, Enter>();
    }

    public void InTo(Func<Room> getRoom)
    {
        AddToRoomMap<Enter>(getRoom);
    }

    public void OutTo<T>() where T : Room
    {
        AddToRoomMap<T, Exit>();
    }

    public void OutTo(Func<Room> getRoom)
    {
        AddToRoomMap<Exit>(getRoom);
    }

    public void UpTo<T>() where T : Room
    {
        AddToRoomMap<T, Up>();
    }

    public void UpTo(Func<Room> getRoom)
    {
        AddToRoomMap<Up>(getRoom);
    }

    public void DownTo<T>() where T : Room
    {
        AddToRoomMap<T, Down>();
    }

    public void DownTo(Func<Room> getRoom)
    {
        AddToRoomMap<Down>(getRoom);
    }

    #endregion

    // used solely for glowing sword
    public List<Room> AdjoiningRooms => [.. _adjoiningRooms.Values];

    #region Used by Player Movement Code

    /// <summary>
    /// Returns room in given direction, used for movement
    /// </summary>
    public Room GetRoomTo(Direction direction)
    {
        var type = direction.GetType();

        if (Vehicle)
        {
            return ((Room)Parent).GetRoomTo(direction);
        }

        Door door = null;

        if (!_roomMap.ContainsKey(type) && !TryFindDoor(type, out door))
        {
            var before = GetBeforeRoutine(type);

            if (before != null && before())
            {
                return null;
            }

            Output.Print(Messages.CantGoThatWay);
            return null;
        }

        Room room = null;

        if (door != null)
        {
            room = door;
        }
        else
        {
            var getRoom = _roomMap[type];
            room = getRoom();
        }

        if (room is Door)
        {
            door = (Door)room;
        }

        if (room != null && door != null)
        {
            room = door.GetOtherSide();
        }

        return room;
    }

    public Room GetRoomTo<T>() where T : Direction
    {
        var direction = Routines.Get<T>();
        return GetRoomTo(direction);
    }

    private bool TryFindDoor(Type type, out Door door)
    {
        door = null;

        foreach (var d in Children.OfType<Door>())
        {
            var doorDir = d.DoorDirection();

            if (doorDir?.GetType() == type)
            {
                door = d;
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Helpers for Room definitions and Action Routines

    // e.g. if (Location.Is<Kitchen>()) { ... }

    public bool Is<A>() where A : Room
    {
        return Is(typeof(A));
    }

    public bool Is<A, B>()
        where A : Room
        where B : Room
    {
        return Is<A>() || Is<B>();
    }

    public bool Is<A, B, C>()
        where A : Room
        where B : Room
        where C : Room
    {
        return Is<A, B>() || Is<C>();
    }

    public bool Is<A, B, C, D>()
        where A : Room
        where B : Room
        where C : Room
        where D : Room
    {
        return Is<A, B>() || Is<C, D>();
    }

    public bool Is<A, B, C, D, E>()
        where A : Room
        where B : Room
        where C : Room
        where D : Room
        where E : Room
    {
        return Is<A, B, C, D>() || Is<E>();
    }

    private bool Is(Type type)
    {
        return Player.Location.GetType() == type;
    }

    #endregion

    #region Scenery

    // This is for adding scenery objects to rooms during initialization
    //
    // IMPORTANT: This does not remove objects from the object tree
    // because this allows many rooms to share the same scenery
    public void WithScenery<R>() where R : Object
    {
        var obj = Objects.Get<R>();
        Children.Add(obj);
    }

    public void WithScenery<R1, R2>() where R1 : Object where R2 : Object
    {
        WithScenery<R1>();
        WithScenery<R2>();
    }

    public void WithScenery<R1, R2, R3>() where R1 : Object where R2 : Object where R3 : Object
    {
        WithScenery<R1, R2>();
        WithScenery<R3>();
    }

    public void WithScenery<R1, R2, R3, R4>() where R1 : Object where R2 : Object where R3 : Object where R4 : Object
    {
        WithScenery<R1, R2, R3>();
        WithScenery<R4>();
    }

    public void WithScenery<R1, R2, R3, R4, R5>()
        where R1 : Object where R2 : Object where R3 : Object where R4 : Object where R5 : Object
    {
        WithScenery<R1, R2, R3, R4>();
        WithScenery<R5>();
    }

    public void WithScenery<R1, R2, R3, R4, R5, R6>()
        where R1 : Object where R2 : Object where R3 : Object where R4 : Object where R5 : Object
        where R6 : Object
    {
        WithScenery<R1, R2, R3, R4, R5>();
        WithScenery<R6>();
    }

    public void WithScenery<R1, R2, R3, R4, R5, R6, R7>()
        where R1 : Object where R2 : Object where R3 : Object where R4 : Object where R5 : Object
        where R6 : Object where R7 : Object
    {
        WithScenery<R1, R2, R3, R4, R5, R6>();
        WithScenery<R7>();
    }

    #endregion
}