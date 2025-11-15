using Zork1.Library;

namespace Zork1.Handlers;

public class Restart : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        Score.PrintScore(false);
        Console.Write($"{Environment.NewLine}Do you wish to restart? (Y is affirmative): ");
        var response = Console.ReadLine()?.ToLower();
        if (response == "yes" || response == "y")
        {
            Console.Clear();
            Context.Story.Initialize();
        }

        return true;
    }
}
