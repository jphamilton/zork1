using System.Text;
using Zork1.Handlers;
using Zork1.Library;
using Zork1.Library.Parsing;
using Zork1.Library.Utilities;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class LoudRoom : Room
{
    private PickOne<Room> Near;

    public bool Echo { get; set; }

    public LoudRoom()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        Near = new([
            Get<DampCave>(),
            Get<RoundRoom>(),
            Get<DeepCanyon>()
        ]);

        Name = "Loud Room";

        Describe = () =>
        {
            StringBuilder sb = new("This is a large room with a ceiling which cannot be detected from the ground. " +
                "There is a narrow passage from east to west and a stone stairway leading upward.");

            if (Echo)
            {
                sb.Append(" The room is eerie in its quietness.");
            }
            else
            {
                sb.Append(" The room is deafeningly loud with an undetermined rushing sound. The sound seems to reverberate " +
                    "from all of the walls, making it difficult even to think.");
            }

            return sb.ToString();
        };

        Initial = () =>
        {
            if (Flags.DamOpen && !Flags.LowTide)
            {
                Print("^It is unbearably loud here, with an ear-splitting roar seeming to come from all around you. " +
                    "There is a pounding in your head which won't stop. With a tremendous effort, you scramble out of the room.");
                GoTo(Nearby());
                return false;
            }

            return true;
        };

        WithScenery<Stairs>();
        IsHere<PlatinumBar>();

        UpTo<DeepCanyon>();
        WestTo<RoundRoom>();
        EastTo<DampCave>();

        Before<Enter>(() => false);

        Before<Echo>(DoEcho);

        Before(() =>
        {
            if (Verb is Direction || Verb is Quit || Verb is Save || Verb is Restore)
            {
                return false;
            }

            if (Echo || (!Flags.DamOpen && Flags.LowTide) || (Flags.DamOpen && !Flags.LowTide))
            {
                return false;
            }

            if (State.Commands > 1)
            {
                State.Commands = 0;
                Print("^The rest of your commands have been lost in the noise.");
            }

            while (true)
            {
                var input = Prompt.GetInput();

                if (string.IsNullOrEmpty(input))
                {
                    Output.Print(Messages.BegYourPardon);
                    continue;
                }

                var command = CommandLine.GetCommands(input)[0];
                var frame = Lexer.Tokenize(command, null);
                SyntaxCheck.Check(frame, out Grammar grammar);

                if (frame.Sub is Save)
                {
                    return Redirect.To<Save>();
                }
                else if (frame.Sub is Restore)
                {
                    return Redirect.To<Restore>();
                }
                else if (frame.Sub is Quit)
                {
                    return Redirect.To<Quit>();
                }
                else if (frame.Sub is West)
                {
                    return GoTo<RoundRoom>();
                }
                else if (frame.Sub is East)
                {
                    return GoTo<DampCave>();
                }
                else if (frame.Sub is Up)
                {
                    return GoTo<DeepCanyon>();
                }
                else if (input == "bug")
                {
                    Print("That's only your opinion.");
                    continue;
                }
                else if (input == "echo")
                {
                    return DoEcho();
                }

                var word = input.Split(' ').Last();
                Print($"{word} {word}...");
            }
        });
    }

    private bool DoEcho()
    {
        var bar = Get<PlatinumBar>();
        Echo = true;
        bar.Sacred = false;
        return Print("The acoustics of the room change subtly.");
    }

    public Room Nearby()
    {
        return Near.Pick();
    }
}
