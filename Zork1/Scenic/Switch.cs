using Zork1.Handlers;
using Zork1.Things;

namespace Zork1.Scenic;

public class Switch : Object
{
    public Switch()
    {
        Scenery = true;
        Turnable = true;
    }

    public override void Initialize()
    {
        Name = "switch";
        Adjectives = ["switch"];

        var machine = Get<Machine>();

        Before<SwitchOn, MoveWith>(() =>
        {
            if (Second is Screwdriver)
            {
                return machine.TurnOn();
            }

            if (Second == null)
            {
                return Print("You can't turn it with your hands...");
            }

            return Print($"It seems that a {Second} won't do.");
        });
    }
}
