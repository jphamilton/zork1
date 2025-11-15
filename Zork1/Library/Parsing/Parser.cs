namespace Zork1.Library.Parsing;

public static class Parser
{
    public static Frame Parse(string command, Frame previous = null)
    {
        var frame = Lexer.Tokenize(command, previous);

        if (frame.IsError)
        {
            return frame;
        }

        if (!SyntaxCheck.Check(frame, out var grammar))
        {
            return frame;
        }

        if (!Snarf.Objects(frame, grammar))
        {
            return frame;
        }

        if (!Many.Check(frame, grammar))
        {
            return frame;
        }

        if (!TakeCheck.Check(frame))
        {
            return frame;
        }

        frame.Sub = grammar.Handler;

        Context.Verb = frame.Sub;

        return frame;
    }
}
