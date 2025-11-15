
namespace Zork1.Library;

[Flags]
public enum WordFlags
{
    None = 0,
    Verb = 1,
    Preposition = 2,
    Object = 4,
    Direction = 8,
}

public static class Dictionary
{
    public static List<string> Verbs { get; private set; } = [];
    public static List<string> Directions { get; private set; } = [];
    public static List<string> Prepositions { get; private set; } = [];
    public static List<string> Objects { get; private set; } = [];

    public static void Load()
    {
        Verbs = [];

        Directions = [
            "north", "south", "east", "west", "northeast", "southeast", "southwest", "northwest",
            "up", "down", "in", "out"
        ];

        Prepositions = [];
        Objects = [];
    }

    public static void Sort()
    {
        Verbs.Sort();
        Directions.Sort();
        Prepositions.Sort();
        Objects.Sort();
    }

    public static bool LookUp(string word, out WordFlags wordFlags)
    {
        wordFlags = WordFlags.None;

        if (Verbs.Contains(word))
        {
            wordFlags |= WordFlags.Verb;
        }

        if (Directions.Contains(word))
        {
            wordFlags |= WordFlags.Direction;
        }

        if (Prepositions.Contains(word))
        {
            wordFlags |= WordFlags.Preposition;
        }

        if (Objects.Contains(word))
        {
            wordFlags |= WordFlags.Object;
        }

        return wordFlags != WordFlags.None;
    }

    public static bool IsPreposition(string word)
    {
        return LookUp(word, out WordFlags f) && (f & WordFlags.Preposition) != 0;
    }

    public static bool IsVerb(string word)
    {
        return LookUp(word, out WordFlags f) && (f & WordFlags.Verb) != 0;
    }

    public static void AddVerb(string verb)
    {
        if (!Verbs.Contains(verb))
        {
            Verbs.Add(verb);
        }
    }

    public static void AddPreposition(string preposition)
    {
        if (!Prepositions.Contains(preposition))
        {
            Prepositions.Add(preposition);
        }
    }

    public static void AddObject(Object obj)
    {
        foreach (var adj in obj.Adjectives)
        {
            if (!Objects.Contains(adj))
            {
                Objects.Add(adj);

                if (Prepositions.Contains(adj))
                {
                    throw new Exception($"Object [{obj.Name}] - '{adj}' is a preposition and cannot be used as an object name.");
                }
            }
        }
    }
}
