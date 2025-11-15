using Zork1.Handlers;
using Zork1.Rooms;

namespace Zork1.Things;

public class GoldenCanary : Object
{
    public GoldenCanary()
    {
        Search = true;
        Takeable = true;
        TakeValue = 6;
        TrophyValue = 4;
    }

    public override void Initialize()
    {
        Name = "golden clockwork canary";
        Adjectives = ["clockw", "gold", "golden", "canary", "treasure"];
        Initial = "There is a golden clockwork canary nestled in the egg. It has ruby eyes and a silver beak. Through a crystal window " +
            "below its left wing you can see intricate machinery inside. It appears to have wound down.";

        Before<WindUp>(() =>
        {
            if (!Flags.SingSong && Location is SongBirdRoom)
            {
                Print("The canary chirps, slightly off-key, an aria from a forgotten opera. From out of the greenery " +
                    "flies a lovely songbird. It perches on a limb just over your head and opens its beak to sing. As it " +
                    "does so a beautiful brass bauble drops from its mouth, bounces off the top of your head, and lands " +
                    "glimmering in the grass. As the canary winds down, the songbird flies away.");
                Flags.SingSong = true;
                var bauble = Get<BrassBauble>();
                var loc = Location.Is<UpATree>() ? Get<ForestPath>() : Location;
                bauble.Move(loc);
                return true;
            }

            return Print("The canary chirps blithely, if somewhat tinnily, for a short time.");
        });
    }
}
