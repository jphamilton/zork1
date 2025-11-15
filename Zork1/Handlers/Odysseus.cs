using Zork1.Library;
using Zork1.Rooms;
using Zork1.Things;

namespace Zork1.Handlers;

public class Odysseus : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        var cyclops = Get<Cyclops>();
        var cyclopsRoom = Get<CyclopsRoom>();

        if (Location == cyclopsRoom && cyclops.Parent == cyclopsRoom && !cyclops.Asleep)
        {
            cyclops.Asleep = true;
            cyclops.Fled = true;
            cyclops.Fight = false;
            cyclops.Remove();
            cyclops.StopDaemon();

            return Print("The cyclops, hearing the name of his father's deadly nemesis, flees the room by knocking down the wall on the east of the room.");
        }

        return Print("Wasn't he a sailor?");
    }
}