using System.Collections.Concurrent;
using System.Diagnostics;

namespace Zork1.Library;

public static class Clock
{
    private static List<Object> _daemons = [];
    private static ConcurrentDictionary<Func<bool>, int> _clock = [];
    private static bool _running;

    public static bool Wait { get; set; }

    public static bool Run(bool noTurn)
    {
        if (Wait)
        {
            Wait = false;
            return false;
        }

        foreach (var obj in _daemons.ToList())
        {
            obj.Daemon();
        }

        List<Func<bool>> due = [];

        foreach (var kvp in _clock)
        {
            var routine = kvp.Key;
            var ticks = kvp.Value;

            if (ticks < 0)
            {
                Debugger.Break();
            }

            if (ticks == 0)
            {
                due.Add(routine);
                continue;
            }

            _clock[routine] = --ticks;
        }

        foreach (var routine in due)
        {
            _running = true;
            _clock.TryRemove(routine, out _);
            routine();
            _running = false;
        }

        if (!noTurn)
        {
            State.Moves++;
        }

        return due.Count > 0;
    }

    public static void Queue(Func<bool> routine, int ticks)
    {
        if (_running)
        {
            ticks--;
        }

        _clock.AddOrUpdate(routine, ticks, (k, v) => ticks);
    }

    public static void Queue<T>(int ticks) where T : Object
    {
        var obj = Objects.Get<T>();
        Queue(obj, ticks);
    }

    public static void Queue(Object obj, int ticks)
    {
        if (obj.Daemon == null)
        {
            throw new Exception($"{obj} has no Daemon defined.");
        }

        Queue(obj.Daemon, ticks);
    }

    public static void Queue<T>() where T : Object
    {
        var obj = Objects.Get<T>();
        Queue(obj);
    }

    public static void Queue(Object obj)
    {
        if (obj.Daemon == null)
        {
            throw new Exception($"{obj.GetType().Name} has no Daemon defined.");
        }

        if (_daemons.Contains(obj))
        {
            return;
        }

        _daemons.Add(obj);
    }

    public static void Interrupt<T>() where T : Object
    {
        var obj = Objects.Get<T>();
        Interrupt(obj);
    }

    public static void Interrupt(Object obj)
    {
        _daemons.Remove(obj);

        if (obj.Daemon != null)
        {
            Interrupt(obj.Daemon);
        }
    }

    public static void Interrupt(Func<bool> routine)
    {
        _clock.TryRemove(routine, out _);
    }

    public static int Ticks(Object obj)
    {
        if (obj.Daemon == null)
        {
            throw new Exception($"{obj.GetType().Name} has no Daemon defined.");
        }

        return Ticks(obj.Daemon);
    }

    public static int Ticks(Func<bool> routine)
    {
        if (_clock.TryGetValue(routine, out int ticks))
        {
            return ticks;
        }

        return 0;
    }
}
