using System.Text;
using Zork1.Library;
using Zork1.Things;

namespace Zork1.Handlers;

public class Debug : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        State.Debug = !State.Debug;

        if (State.Debug)
        {
            Print("Debug is on.");
        }
        else
        {
            Print("Debug is off.");
        }

        return true;
    }

}

public abstract class DebugSub : Sub
{
    protected Object GetObject(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return null;
        }

        line = line.ToLowerInvariant();

        var words = line.Split(' ').ToList();
        List<Object> found = [];

        if (words.Count > 0)
        {
            found = [.. Objects.All.Where(x => x.Adjectives.Contains(words[0]))];
        }

        for (var i = 1; i < words.Count; i++)
        {
            found = [.. found.Where(x => x.Adjectives.Contains(words[i]))];
        }

        if (found.Count == 1)
        {
            return found[0];
        }

        return null;
    }
}

public class Purloin : DebugSub
{
    public override bool Handler(Object noun, Object second)
    {
        Console.Write(": ");
        var line = Console.ReadLine();
        var obj = GetObject(line);

        if (obj != null)
        {
            obj.Move(player);
            Score.ScoreObject(obj);
            return Print("(Purloined)");
        }

        return Print("Failed.");
    }
}

public class GoNear : DebugSub
{
    public override bool Handler(Object noun, Object second)
    {
        Console.Write(": ");
        var line = Console.ReadLine();

        var obj = Objects.All.FirstOrDefault(x => string.Equals(x.Name, line, StringComparison.CurrentCultureIgnoreCase));

        if (obj != null)
        {
            while (obj != null && obj is not Room)
            {
                obj = obj.Parent;
            }

            if (obj != null)
            {
                MovePlayer.To((Room)obj);
                return true;
            }

        }

        obj = GetObject(line);

        if (obj != null && obj.Parent is Room)
        {
            MovePlayer.To((Room)obj.Parent);
        }

        return true;

    }
}

public class Replay : DebugSub
{
    public override bool Handler(Object noun, Object second)
    {
        Console.Write(": ");
        var file = Console.ReadLine();
        var dir = Path.GetDirectoryName(Context.Story.GetType().Assembly.Location);
        var path = Path.Combine(dir, file);

        if (!File.Exists(path))
        {
            Print("Replay file not found.");
            return true;
        }

        var thief = Get<Thief>();
        thief.StopDaemon();

        var fakeOutput = new StringBuilder();
        Output.Initialize(new StringWriter(fakeOutput), new WordWrap(80));

        var commands = File.ReadAllLines(path).Where(x => !string.IsNullOrEmpty(x)).ToList();
        string error = null;

        Console.Write("> ");

        foreach (var command in commands)
        {
            try
            {
                Console.WriteLine(command);
                //Thread.Sleep(200);

                if (command.StartsWith("attack"))
                {
                    Attack(command, fakeOutput, out error);
                    continue;
                }

                MainLoop.CommandRun(command, out error);

                if (string.IsNullOrEmpty(fakeOutput.ToString()))
                {
                    Console.WriteLine("no response");
                    break;
                }

                if (fakeOutput.ToString().Contains("red buoy"))
                {
                    break;
                }

                if (command == "odysseus")
                {
                    thief.StartDaemon();
                }

                if (command.StartsWith("take") && !fakeOutput.ToString().Contains("Taken."))
                {
                    Console.WriteLine("Replay error, stopping.");
                    break;
                }

                Console.Write(fakeOutput);
                fakeOutput.Clear();
            }
            catch (DeathException dex)
            {
                Console.Write(fakeOutput);
                fakeOutput.Clear();

                Print(dex.Message);
                break;
            }
            catch (Exception)
            {
                Console.Write(fakeOutput);
                fakeOutput.Clear();

                throw;
            }

            if (command != commands[^1])
            {
                Console.WriteLine();
                Console.Write("> ");
            }

            //Thread.Sleep(200);
        }

        Console.Write(fakeOutput);
        fakeOutput.Clear();

        Output.Initialize(Console.Out, new WordWrap(80));

        if (error != null)
        {
            Print(error);
        }

        return true;
    }

    private void Attack(string command, StringBuilder output, out string error)
    {
        error = null;

        while (!output.ToString().Contains("the carcass has disappeared"))
        {
            output.AppendLine();
            output.AppendLine($"> {command}");
            MainLoop.CommandRun(command, out error);
        }

    }
}