using Zork1.Library;
using Zork1.Things;

namespace Zork1.Handlers;

public class Quit : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        return Run();
    }

    public static bool Run(bool askFirst = true)
    {
        Score.PrintScore(askFirst);
        Console.Write("^Do you wish to leave the game? (Y is affirmative): ");
        var response = Console.ReadLine()?.ToLower();
        if (response != "yes" && response != "y")
        {
            Print("Ok.");
        }
        else
        {
            Flags.Done = true;
        }

        return true;
    }
}