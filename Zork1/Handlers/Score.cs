using System.Text;
using Zork1.Library;
using Zork1.Rooms;
using Zork1.Things;

namespace Zork1.Handlers;

public class Score : Sub
{
    public Score()
    {
        NoTurn = true;
    }

    public override bool Handler(Object noun, Object second)
    {
        return PrintScore(false);
    }

    public static void AddTrophy(Object obj)
    {
        Output.Debug($"{obj} +{obj.TrophyValue}");
        Update(obj.TrophyValue);
    }

    public static void RemoveTrophy(Object obj)
    {
        Output.Debug($"{obj} -{obj.TrophyValue}");
        Update(-obj.TrophyValue);
    }

    public static void ScoreObject(Object obj)
    {
        var value = obj.TakeValue;

        if (value <= 0)
        {
            return;
        }

        Output.Debug($"{obj.Name} +{value}");
        Update(value);
        obj.TakeValue = 0;
    }

    public static bool PrintScore(bool ask)
    {
        var tense = ask ? "would be" : "is";
        var moves = State.Moves == 1 ? "move" : "moves";
        var score = State.Score;

        var sb = new StringBuilder();

        sb.Append($"Your score {tense} {score} (total of 350 points), in {State.Moves} {moves}.");

        var rank = "Beginner";

        if (score == 350)
        {
            rank = "Master Adventurer";
        }
        else if (score > 330)
        {
            rank = "Wizard";
        }
        else if (score > 300)
        {
            rank = "Master";
        }
        else if (score > 200)
        {
            rank = "Adventurer";
        }
        else if (score > 100)
        {
            rank = "Junior Adventurer";
        }
        else if (score > 50)
        {
            rank = "Novice Adventurer";
        }
        else if (score > 25)
        {
            rank = "Amateur Adventurer";
        }
        else
        {
            rank = "Beginner";
        }

        sb.Append($"^This score gives you the rank of {rank}.");

        Print(sb.ToString());

        return true;
    }

    public static void Update(int value)
    {
        State.Score += value;

        if (State.Score != 350 || Flags.Won)
        {
            return;
        }

        Flags.Won = true;

        Get<AncientMap>().Concealed = false;
        Get<WestOfHouse>().Visited = false;
    }
}
