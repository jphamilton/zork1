using Zork1.Handlers;
using Zork1.Library.Things;
using Zork1.Rooms;

namespace Zork1.Things;

public class Cyclops : Villain
{
    public int Cyclowrath { get; set; }
    public bool Fled { get; set; }
    public bool Asleep { get; set; }

    private readonly List<string> CycloMessages = [
        "^The cyclops seems somewhat agitated.",
        "^The cyclops appears to be getting more agitated.",
        "^The cyclops is moving about the room, looking for something.",
        "^The cyclops was looking for salt and pepper. No doubt they are condiments for his upcoming snack.",
        "^The cyclops is moving toward you in an unfriendly manner.",
        "^You have two choices: 1. Leave  2. Become dinner."
    ];

    public Cyclops()
    {
        Animate = true;
        Asleep = false;
        Scenery = true;
        Strength = 10000;
        TryTake = true;
    }

    public override void Initialize()
    {
        Name = "cyclops";
        Adjectives = ["cyclops", "monster", "eye", "hungry", "giant"];

        Describe = () =>
        {
            var desc = "";

            if (Asleep && !Fled)
            {
                return desc += "The cyclops is sleeping blissfully at the foot of the stairs.";
            }

            if (Cyclowrath == 0)
            {
                return desc += "A cyclops, who looks prepared to eat horses (much less mere adventurers), blocks the staircase. From his state of health, and the bloodstains on the walls, you gather that he is not very friendly, though he likes people.";
            }
            else if (Cyclowrath > 0)
            {
                return desc += "The cyclops is standing in the corner, eyeing you closely. I don't think he likes you very much. He looks extremely hungry, even for a cyclops.";
            }

            // Cyclowrath < 0
            return desc += "The cyclops, having eaten the hot peppers, appears to be gasping. His enflamed tongue protrudes from his man-sized mouth.";
        };

        Daemon = () =>
        {
            if (Asleep || Flags.Dead)
            {
                return true;
            }

            if (!Location.Is<CyclopsRoom>())
            {
                return true;
            }

            if (Cyclowrath < -5 || Cyclowrath > 5)
            {
                StopDaemon();
                JigsUp("The cyclops, tired of all of your games and trickery, grabs you firmly. As he licks his chops, he says ~Mmm. Just like Mom used to make 'em.~ It's nice to be appreciated.");
            }

            Cyclowrath = Cyclowrath < 0 ? --Cyclowrath : ++Cyclowrath;
            var index = Math.Abs(Cyclowrath) - 1;

            return Print(CycloMessages[index]);
        };

        // See Odysseus.cs

        Before<Tell, Answer>(() =>
        {
            if (Asleep)
            {
                return Print("No use talking to him. He's fast asleep.");
            }

            return Print("The cyclops prefers eating to making conversation.");
        });

        Before<Take>(() => Print("The cyclops doesn't take kindly to being grabbed."));
        Before<Listen>(() => Print("You can hear his stomach rumbling."));
        Before<TieTo>(() => Print("You cannot tie the cyclops, though he is fit to be tied."));

        Before<Examine>(() =>
        {
            if (Asleep)
            {
                return Print("The cyclops is sleeping like a baby, albeit a very ugly one.");
            }

            return Print("A hungry cyclops is standing at the foot of the stairs.");
        });

        Before<Give>(() =>
        {
            if (Noun is Lunch lunch)
            {
                Cyclowrath = Math.Min(-1, -Cyclowrath);
                lunch.Remove();
                StartDaemon();
                return Print("The cyclops says ~Mmm Mmm. I love hot peppers! But oh, could I use a drink. Perhaps I could drink the blood of that thing.~" +
                    "  From the gleam in his eye, it could be surmised that you are ~that thing~.");
            }

            var bottle = Get<GlassBottle>();
            var water = Get<QuantityOfWater>();

            if (Noun == water || Noun == bottle && bottle.Has(water))
            {
                if (Cyclowrath < 0)
                {
                    water.Remove();
                    bottle.MoveHere();
                    bottle.Open = true;
                    Fight = false;
                    Asleep = true;
                    return Print("The cyclops takes the bottle, checks that it's open, and drinks the water. A moment later, he lets out a yawn that nearly blows you over, and then falls fast asleep (what did you put in that drink, anyway?).");
                }

                return Print("The cyclops apparently is not thirsty and refuses your generous offer.");
            }

            if (Noun is CloveOfGarlic)
            {
                return Print("The cyclops may be hungry, but there is a limit.");
            }

            return Print("The cyclops is not so stupid as to eat THAT!");
        });

        Before<Poke>(() =>
        {
            StartDaemon();
            return Print($"~Do you think I'm as stupid as my father was?~, he says, dodging.");
        });

        Before<Attack>(() =>
        {
            StartDaemon();
            return Print("The cyclops shrugs but otherwise ignores your pitiful attempt.");
        });

        Before<Throw>(() =>
        {
            StartDaemon();
            Noun.MoveHere();
            return Print("The cyclops shrugs but otherwise ignores your pitiful attempt.");
        });

        Before<Wake>(() =>
        {
            if (!Asleep)
            {
                return false;
            }

            Asleep = false;
            Fight = true;

            StartDaemon();

            // set cyclops to "has not eaten hot peppers" state
            if (Cyclowrath < 0)
            {
                Cyclowrath = 0 - Cyclowrath;
            }

            return Print("The cyclops yawns and stares at the thing that woke him up.");
        });
    }
}