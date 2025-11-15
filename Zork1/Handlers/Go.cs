using Zork1.Library;

namespace Zork1.Handlers;

public class Go : Sub
{
    // I am handling Go a little differently.
    // The directions themselves are action routines, so
    // commands like "go north" are resolved to "north".
    // Commands like "go in window" are resolved to "enter window".
    // This is just a stub that tells the parser Go is a valid
    // verb.
    public override bool Handler(Object noun, Object second)
    {
        return false;
    }
}
