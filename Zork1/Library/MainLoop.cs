using Zork1.Handlers;
using Zork1.Library.Parsing;
using Zork1.Library.Things;
using Zork1.Things;

namespace Zork1.Library;

public class MainLoop
{
    private static Frame _previous;
    private static Frame _lastAction;

    public MainLoop(Story story)
    {
        AppDomain.CurrentDomain.ProcessExit += (s, e) => Output.StopScripting();

        Console.Title = story.Title;
        Output.Initialize(Console.Out, new WordWrap(80));
        Prompt.Initialize(Console.In);
        Context.Story = story ?? throw new ArgumentNullException(nameof(story));
    }

    public static void Run()
    {
        Context.Story.Initialize();

        while (!Flags.Done)
        {
            try
            {
                var input = Prompt.GetInput();

                if (string.IsNullOrEmpty(input))
                {
                    Output.Print(Messages.BegYourPardon);
                    continue;
                }

                // split input into one or more commands
                var commands = CommandLine.GetCommands(input);

                State.Commands = commands.Count;

                string error = null;

                foreach (var command in commands)
                {
                    if (!CommandRun(command, out error))
                    {
                        break;
                    }

                    State.Commands--;

                    if (State.Commands <= 0)
                    {
                        break;
                    }
                }

                if (error != null)
                {
                    Output.Print(error);
                }
            }
            catch (DeathException dex)
            {
                Output.Print(dex.Message);
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(true);
    }

    public static bool CommandRun(string command, out string error)
    {
        error = null;

        var room = Player.Location;

        // do we have light?
        State.Lit = Query.Light(room);
        var wasLit = State.Lit;

        var frame = Parser.Parse(command, _previous);

        if (frame.IsError || frame.Orphan)
        {
            _lastAction = null;
            error = frame.Error;
            _previous = frame;
            return false;
        }

        _previous = null;

        Frame f = null;

        bool isAgain = frame.Sub is Again;

        if (isAgain)
        {
            if (_lastAction != null)
            {
                f = _lastAction;
            }
            else
            {
                Output.Print(Messages.BegYourPardon);
                return false;
            }
        }
        else
        {
            f = frame;
        }

        var action = new Command(f);

        var status = action.Run();

        if (status.Success && !isAgain)
        {
            _lastAction = f;
        }

        // check if lights came on or off or if we moved into a dark room
        CurrentRoom.CheckLight(room, wasLit);

        // run daemons
        Clock.Run(frame.Sub.NoTurn);

        return true;
    }
}
