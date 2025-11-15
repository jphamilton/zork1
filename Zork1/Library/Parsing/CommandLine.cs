
namespace Zork1.Library.Parsing;

/// <summary>
/// Takes input from command prompt and resolves it to
/// one or more normalized command strings. Stop words are
/// stripped.
/// </summary>
public static class CommandLine
{
    public static List<string> GetCommands(string input)
    {
        // start with some basic normalization
        input = input.Trim().ToLowerInvariant().Replace(".", " then ").Replace(",", " and ");

        // NOTE: input could be "take bottle and lamp" which is 1 command
        // or "open mailbox and read leaflet" which is 2 commands
        var tokens = input.Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToList();

        if (tokens.Count == 0)
        {
            return [];
        }

        List<string> current = [];
        List<string> results = [];
        var hasVerb = Dictionary.Verbs.Contains(tokens[0]);
        string lastToken = null;

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i].ToLowerInvariant().Trim();

            var isVerb = Dictionary.Verbs.Contains(token);

            if (i > 0 && current.Count > 0 && isVerb && (lastToken == "then" || lastToken == "and"))
            {
                results.Add(string.Join(' ', current));
                current = [];
            }

            lastToken = token;

            if (!IgnoredWords.Contains(token))
            {
                current.Add(token);
            }
        }

        results.Add(string.Join(' ', current));

        return results;
    }
}
