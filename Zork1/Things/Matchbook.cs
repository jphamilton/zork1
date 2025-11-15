using Zork1.Handlers;
using Zork1.Library;
using Zork1.Rooms;

namespace Zork1.Things;

public class Matchbook : Object
{
    public int Count { get; set; } = 6;

    public Matchbook()
    {
        Readable = true;
        Takeable = true;
        Size = 2;
    }

    public override void Initialize()
    {
        Name = "matchbook";
        Adjectives = ["match", "matches", "matchbook"];
        Description = "There is a matchbook whose cover says ~Visit Beautiful FCD#3~ here.";
        Text = "^(Close cover before striking)^^YOU too can make BIG MONEY in the exciting field of PAPER SHUFFLING!^^" +
            "Mr. Anderson of Muddle, Mass. says: ~Before I took this course I was a lowly bit twiddler. Now with what I " +
            "learned at GUE Tech I feel really important and can obfuscate and confuse with the best.~^^" +
            "Dr. Blank had this to say: ~Ten short days ago all I could look forward to was a dead-end job as a doctor. " +
            "Now I have a promising future and make really big Zorkmids.~^^" +
            "GUE Tech can't promise these fantastic results to everyone. But when you earn your degree from GUE Tech, your future will be brighter.";

        Before<Burn, SwitchOn>(() =>
        {
            if (Noun != this)
            {
                return false;
            }

            --Count;

            if (Count <= 0)
            {
                return Print("I'm afraid that you have run out of matches.");
            }

            if (Location is Drafty)
            {
                return Print("This room is drafty, and the match goes out instantly.");
            }

            Flame = true;
            Light = true;

            Clock.Queue(MatchDaemon, 2);

            Print("^One of the matches starts to burn.");

            return true;
        });

        Before<SwitchOff>(() =>
        {
            Print("^The match is out");
            Flame = false;
            Light = false;
            Lit = Query.Light(Location);

            if (!Lit)
            {
                Print("It's pitch black in here!");
            }

            Clock.Queue(MatchDaemon, 0);

            return true;
        });

        Before<Open, Count>(() =>
        {
            var matches = Count > 1 ? "matches" : "match";
            return Print($"You have {Count} {matches} left.");
        });

        Before<Examine>(() =>
        {
            if (Light)
            {
                return Print("The match is burning.");
            }

            return Print("The matchbook isn't very interesting, except for what's written on it.");
        });
    }

    private bool MatchDaemon()
    {
        Print("^The match has gone out.");
        Flame = false;
        Light = false;
        Lit = Query.Light(Location);
        return true;
    }
}
