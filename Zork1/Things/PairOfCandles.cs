using Zork1.Handlers;
using Zork1.Library;
using Zork1.Library.Things;

namespace Zork1.Things;

public class PairOfCandles : Object
{
    public int TimeLeft { get; set; } = 22;

    public PairOfCandles()
    {
        Size = 10;
        Takeable = true;
        Light = true;
        Flame = true;
        On = true;
        Switchable = true;
    }

    public override void Initialize()
    {
        Name = "pair of candles";
        Adjectives = ["candle", "candles", "pair", "burning"];
        Initial = "On the two ends of the altar are burning candles.";

        Daemon = () =>
        {
            if (!Flame)
            {
                return true;
            }

            TimeLeft--;

            if (Player.Has(this))
            {
                if (TimeLeft == 20)
                {
                    Print("^The candles grow shorter.");
                }

                if (TimeLeft == 10)
                {
                    Print("^The candles are becoming quite short.");
                }

                if (TimeLeft == 5)
                {
                    Print("^The candles won't last long now.");
                }

                if (TimeLeft == 0)
                {
                    Print("^You'd better have more light than from the pair of candles.");
                }

                if (TimeLeft <= 0)
                {
                    StopDaemon();
                    Light = false;
                    On = false;
                    Flame = false;
                    RMUNGBIT = true;
                }
            }

            return true;
        };

        Before<Take>(() =>
        {
            if (Light && !Visited)
            {
                StartDaemon();
            }

            return false;
        });

        Before<Burn, SwitchOn>(() =>
        {
            // https://github.com/the-infocom-files/zork1/issues/55
            if (Second == this)
            {
                return false;
            }

            var matchbook = Get<Matchbook>();
            var torch = Get<Torch>();

            if (RMUNGBIT)
            {
                return Print("Alas, there's not much left of the candles. Certainly not enough to burn.");
            }

            if (Second == null)
            {

                if (Player.Has(matchbook) && matchbook.Flame)
                {
                    Print("(with the match)");
                    return Redirect.To<SwitchOn>(this, matchbook);
                }
            }

            if (Second == matchbook && matchbook.Light)
            {
                var desc = "The candles are ";
                if (Light)
                {
                    desc += "already lit.";
                    return Print(desc);
                }

                // https://github.com/the-infocom-files/zork1/issues/55
                Flame = true;
                Light = true;
                On = true;

                desc += "lit.";
                StartDaemon();

                return Print(desc);
            }

            if (Second == torch)
            {
                if (Light)
                {
                    return Print("You realize, just in time, that the candles are already lighted.");
                }

                Remove();

                return Print("The heat from the torch is so intense that the candles are vaporized.");
            }

            return Print("You have to light them with something that's burning, you know.");
        });

        Before<SwitchOff>(() =>
        {
            StopDaemon();

            if (Light)
            {
                Print("The flame is extinguished.");

                // https://github.com/the-infocom-files/zork1/issues/55
                Flame = false;
                Light = false;
                On = false;

                Lit = Query.Light(Location);

                if (!Lit)
                {
                    return Print("It's really dark in here....");
                }

                return true;
            }

            return Print("The candles are not lit.");
        });

        Before<Insert>(() =>
        {
            if (Second.Flammable)
            {
                return Print("That wouldn't be smart.");
            }

            return false;
        });

        Before<Examine>(() =>
        {
            var state = Light ? "burning" : "out";
            return Print($"The candles are {state}");
        });

        Before<Count>(() => Print("Let's see, how many objects in a pair? Don't tell me, I'll get it."));
    }
}