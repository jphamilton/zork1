using Zork1.Handlers;
using Zork1.Library;
using Zork1.Scenic;

namespace Zork1.Rooms;

public class GratingRoom : Room
{
    public GratingRoom()
    {
        DryLand = true;
    }

    public override void Initialize()
    {
        var grating = Get<Grating>();

        Name = "Grating Room";

        Describe = () =>
        {
            var desc = "You are in a small room near the maze. There are twisty passages in the immediate vicinity.^";
            if (grating.Open)
            {
                desc += "^Above you is an open grating with sunlight pouring in.";
            }
            else if (!grating.Locked)
            {
                desc += "^Above you is a grating.";
            }
            else
            {
                desc += "^Above you is a grating locked with a skull-and-crossbones lock.";
            }

            return desc;
        };

        WithScenery<Grating>();

        SouthWestTo<Maze13>();

        UpTo(() =>
        {
            if (grating.Open)
            {
                return Get<Clearing1>();
            }

            Print("The grating is closed.");

            return this;
        });

        Before<Enter>(() =>
        {
            grating.Concealed = false;
            return false;
        });
    }
}