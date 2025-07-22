using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Rooms;
using Zork1.Melee;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1;

public class Zork1GUE : Story
{
    public Zork1GUE() : base(new ZorkSyntax())
    {
        Name = "Zork I: The Great Underground Empire";
        Title = "Zork I";
    }

    protected override void Start()
    {
        Output.Print(Messages.Version);

        State.Verbose = false;
        State.SuperBrief = false;

        MeleeDefinitions.Initialize();

        Clock.Queue<Sword>();
        Clock.Queue<FightDaemon>();
        Clock.Queue<Thief>();

        var adventurer = Objects.Get<Adventurer>();
        Player.Set(adventurer);
        Context.Winner = adventurer; // haven't implemented this yet

        MovePlayer.To<WestOfHouse>();

        SetLast.Object<Mailbox>();
    }
}
