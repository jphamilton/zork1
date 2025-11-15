using Zork1.Handlers;
using Zork1.Library.Things;

namespace Zork1.Things;

public class BrassLantern : Object
{
    public int TimeLeft { get; set; } = 200;

    public BrassLantern()
    {
        Size = 15;
        Takeable = true;
        Switchable = true;
        Light = false;
        On = false;
    }

    public override void Initialize()
    {
        Name = "brass lantern";
        Adjectives = ["lamp", "lantern", "light", "brass"];
        Description = "There is a brass lantern (battery-powered) here.";
        Initial = "A battery-powered brass lantern is on the trophy case.";

        Before<Throw>(() =>
        {
            Print("The lamp has smashed into the floor, and the light has gone out.");
            StopDaemon();
            On = false;
            Light = false;
            Remove();
            MoveHere<BrokenLantern>();
            return true;
        });

        Before<SwitchOn>(() =>
        {
            if (TimeLeft <= 0)
            {
                return Print("A burned-out lamp won't light.");
            }

            return false;
        });

        Before<SwitchOff>(() =>
        {
            if (TimeLeft <= 0)
            {
                return Print("The lamp has already burned out.");
            }

            return false;
        });

        Before<Examine>(() =>
        {
            var status = TimeLeft <= 0 ? "has burned out" : On ? "is on" : "is turned off";
            return Print($"The lamp {status}.");
        });

        After<SwitchOn>(() =>
        {
            Light = true;
            StartDaemon();
        });

        After<SwitchOff>(() =>
        {
            Light = false;
            StopDaemon();
        });

        Daemon = () =>
        {
            if (!On)
            {
                return true;
            }

            TimeLeft--;

            if (Player.Has(this))
            {
                if (TimeLeft == 100)
                {
                    Print("The lamp appears a bit dimmer.");
                }

                if (TimeLeft == 70)
                {
                    Print("The lamp is definitely dimmer now.");
                }

                if (TimeLeft == 15)
                {
                    Print("The lamp is nearly out.");
                }

                if (TimeLeft == 0)
                {
                    Print("You'd better have more light than from the brass lantern.");
                }
            }

            if (TimeLeft <= 0)
            {
                StopDaemon();
                Light = false;
                On = false;
                Switchable = false;
                RMUNGBIT = true;
            }

            return true;
        };
    }
}
