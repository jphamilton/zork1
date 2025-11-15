using System.Text;
using Zork1.Handlers;
using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class LivingRoom : AboveGround
{
    public override void Initialize()
    {
        Name = "Living Room";
        WithScenery<WhiteHouse, Stairs, TrapDoor, PseudoNails, TrophyCase, WoodenDoor, Carpet>();
        IsHere<Sword>();
        IsHere<BrassLantern>();

        var (cyclops, trapdoor, trophy_case) = Get<Cyclops, TrapDoor, TrophyCase>();

        Describe = () =>
        {
            var trapdoor = Get<TrapDoor>();

            StringBuilder desc = new();

            if (cyclops.Fled)
            {
                desc.Append("You are in the living room. There is a doorway to the east. To the west is a cyclops-shaped opening in " +
                "an old wooden door, above which is some strange gothic lettering, ");
            }
            else
            {
                desc.Append("You are in the living room. There is a doorway to the east, a wooden door with strange gothic lettering to " +
                "the west, which appears to be nailed shut, ");
            }

            desc.Append("a trophy case, ");

            if (Flags.RugMoved && trapdoor.Open)
            {
                desc.Append("and a rug lying beside an open trap door.");
            }
            else if (Flags.RugMoved)
            {
                desc.Append("and a closed trap door at your feet.");
            }
            else if (trapdoor.Open)
            {
                desc.Append("and an open trap door at your feet.");
            }
            else
            {
                desc.Append("and a large oriental rug in the center of the room.");
            }

            return desc.ToString();
        };

        Described = () =>
        {
            if (trophy_case.Items.Count > 0)
            {
                Print("^Your collection of treasures consists of:");
                Print(Describer.DisplayList(trophy_case));
            }
        };

        EastTo<Kitchen>();

        Before<Down>(() =>
        {
            if (Flags.RugMoved)
            {
                var trapdoor = Get<TrapDoor>();

                if (trapdoor.Open)
                {
                    return false;
                }

                SetLast.Object(trapdoor);
                return Print("The trap door is closed.");
            }

            return Print("You can't go that way.");
        });

        Before<West>(() =>
        {
            if (cyclops.Fled)
            {
                return GoTo<StrangePassage>();
            }

            return Print("The door is nailed shut.");
        });

        Before<Take>(() =>
        {
            if (Noun.Parent is TrophyCase && Noun.TrophyValue > 0)
            {
                Score.RemoveTrophy(Noun);
            }

            return false;
        });

        Before<Insert>(() =>
        {
            if (Second is not TrophyCase)
            {
                return false;
            }

            if (Player.Has(Noun) && Noun.TrophyValue > 0)
            {
                Score.AddTrophy(Noun);
            }

            return false;
        });
    }
}
