using Zork1.Handlers;
using Zork1.Library;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Rooms;

public class UpATree : SongBirdRoom
{
    public override void Initialize()
    {
        base.Initialize();

        Name = "Up a Tree";

        Describe = () =>
        {
            var path = Get<ForestPath>();
            var objects = path.Items;

            var msg = "You are about 10 feet above the ground nestled among some large branches. The nearest branch above you is above your reach.";

            if (objects.Count > 0)
            {
                msg += $" On the ground below you can see {Display.List(objects)}.";
            }

            return msg;
        };

        WithScenery<Tree, Forest, Songbird, WhiteHouse>();

        var birds_nest = IsHere<BirdsNest>();
        var (jeweled_egg, broken_egg, forest_path) = Get<JeweledEgg, BrokenEgg, ForestPath>();

        DownTo<ForestPath>();
        Before<Up>(() => Print("You cannot climb any higher."));
        Before<Dive>(() =>
        {
            Print("In a feat of unaccustomed daring, you manage to land on your feet without killing yourself.");
            Move<ForestPath>();
            return true;
        });

        Before<Drop>(() =>
        {
            if (Noun == birds_nest && birds_nest.Has(jeweled_egg))
            {
                birds_nest.Move(forest_path);
                jeweled_egg.Move(forest_path);
                jeweled_egg.BreakEgg();
                return Print("The nest falls to the ground, and the egg spills out of it, seriously damaged.");
            }

            if (Noun == jeweled_egg)
            {
                Print("The egg falls to the ground and springs open, seriously damaged.");
                jeweled_egg.Move(forest_path);
                jeweled_egg.BreakEgg();
                return true;
            }

            Noun.Move<ForestPath>();

            return Print($"The {Noun} falls to the ground.");
        });
    }
}
