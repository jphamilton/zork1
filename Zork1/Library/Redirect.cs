namespace Zork1.Library;

// Redirect from one Routine to another
public static class Redirect
{
    public static bool To<T>(Object obj) where T : Sub, new()
    {
        return To<T>(obj, null);
    }

    public static bool To<T>(Object obj, Object indirect) where T : Sub, new()
    {
        T routine = new();
        Context.Verb = routine;
        return Command.Run<T>(obj, indirect, routine.Handler);

    }

    public static bool To<T>() where T : Sub, new()
    {
        T routine = new();
        Context.Verb = routine;
        return Command.Run<T>(routine.Handler);
    }
}