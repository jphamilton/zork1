namespace Zork1.Library;

public static class Prompt
{
    private static TextReader _input;
    private static List<StringReader> _fake = [];

    public static void Initialize(TextReader input)
    {
        _input = input;
        _fake = [];
    }

    public static string GetInput()
    {
        Output.Write($"{Environment.NewLine}> ");

        if (_fake.Count > 0)
        {
            _input = _fake[0];
            _fake.RemoveAt(0);
        }

        var input = _input.ReadLine()?.Trim();
        Output.ScriptWrite(input);
        return input;
    }

    public static void FakeInput(string text)
    {
        _fake.Add(new StringReader(text));
    }
}