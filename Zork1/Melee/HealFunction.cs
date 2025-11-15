using Zork1.Library;

namespace Zork1.Melee;

public class HealFunction : Object
{
    public override void Initialize()
    {
        Daemon = () =>
        {
            var strength = player.Strength;

            if (strength > 0)
            {
                strength = 0;
                player.Strength = strength;
            }
            else
            {
                strength++;
                player.Strength = strength;
            }

            if (strength < 0)
            {
                if (State.LoadMax < State.LoadAllowed)
                {
                    State.LoadMax += 30;
                }

                Clock.Queue(this, 30);
                return true;
            }

            State.LoadMax = State.LoadAllowed;

            Clock.Interrupt(this);

            return true;
        };
    }
}