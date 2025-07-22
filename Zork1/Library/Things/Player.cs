using System.Diagnostics;

namespace Zork1.Library.Things;

[DebuggerDisplay("(yourself) in {Location}")]
public static class Player
{
    private static Object _player;

    public static Object Instance => _player;

    public static void Set(Object player)
    {
        _player = player;
    }

    public static Object Parent => Instance.Parent;

    public static bool InVehicle => _player.Parent != null && _player.Parent.Vehicle;

    public static bool JigsUp(string message) => _player.JigsUp(message);

    public static Room Location
    {
        get
        {
            if (_player.Parent == null)
            {
                return null;
            }

            return _player.Parent.Vehicle ? (Room)_player.Parent.Parent : (Room)_player.Parent;
        }
        set
        {
            _player.Remove();
            _player.Parent = value;
            _player.Parent.Children.Add(_player);
        }
    }

    public static bool Has(Object obj)
    {
        return _player.Has(obj);
    }

    public static bool Has<T>() where T : Object
    {
        var obj = Objects.Get<T>();
        return _player.Has(obj);
    }

    public static void Add(Object obj)
    {
        obj.Move(_player);
        obj.Visited = true;
    }

    public static T Add<T>() where T : Object
    {
        Object obj = Objects.Get<T>();
        Add(obj);
        return (T)obj;
    }

    public static List<Object> Children => [.. _player.Children];

    public static int Strength => _player.Strength;
    public static int FightStrength(bool adjustForCurrentStrength)
    {
        const int STRENGTH_MAX = 7;
        const int STRENGTH_MIN = 2;

        int baseStrength = STRENGTH_MIN;

        if (State.PossibleScore > 0)
        {
            baseStrength += (int)Math.Floor((double)State.Score / State.PossibleScore * (STRENGTH_MAX - STRENGTH_MIN));
        }

        if (baseStrength > STRENGTH_MAX)
        {
            baseStrength = STRENGTH_MAX;
        }

        if (baseStrength < STRENGTH_MIN)
        {
            baseStrength = STRENGTH_MIN;
        }

        if (adjustForCurrentStrength)
        {
            return baseStrength + Player.Instance.Strength;
        }

        return baseStrength;
    }
}
