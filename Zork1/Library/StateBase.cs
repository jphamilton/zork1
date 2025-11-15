using Zork1.Library.Things;
using Zork1.Things;

namespace Zork1.Library;

public static class Last
{
    private static GameState State => Objects.Get<GameState>();
    public static Object Noun { get => State.LastNoun; set => State.LastNoun = value; }
    public static Object NounPlace { get => State.LastNounPlace; set => State.LastNounPlace = value; }
}

public static class State
{
    private static GameState _State => Objects.Get<GameState>();

    public static int Deaths { get => _State.Deaths; set => _State.Deaths = value; }
    public static bool Debug { get; set; }
    public static bool Lit { get => _State.Lit; set => _State.Lit = value; }
    public static bool SuperBrief { get => _State.SuperBrief; set => _State.SuperBrief = value; }
    public static bool Verbose { get => _State.Verbose; set => _State.Verbose = value; }
    public static int Commands { get; set; }
    public static int LoadAllowed { get => _State.LoadAllowed; set => _State.LoadAllowed = value; }
    public static int LoadMax { get => _State.LoadMax; set => _State.LoadMax = value; }
    public static int MaxHeldMult { get => _State.MaxHeldMult; set => _State.MaxHeldMult = value; }
    public static int MaximumHeld { get => _State.MaximumHeld; set => _State.MaximumHeld = value; }
    public static int Moves { get => _State.Moves; set => _State.Moves = value; }
    public static int PossibleScore { get => _State.PossibleScore; set => _State.PossibleScore = value; }
    public static int Score { get => _State.Score; set => _State.Score = value; }
}

public static class SetLast
{
    public static void Object<T>() where T : Object
    {
        Last.Noun = Objects.Get<T>();
        Last.NounPlace = Player.Location ?? Last.Noun?.Parent;
    }

    public static bool Object(Object obj)
    {
        Last.Noun = obj;
        Last.NounPlace = Player.Location;
        return true;
    }
}