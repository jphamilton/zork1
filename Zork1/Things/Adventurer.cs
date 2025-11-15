using Zork1.Handlers;
using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Rooms;

namespace Zork1.Things;

public class Adventurer : Object
{
    public Adventurer()
    {
        Animate = true;
        Strength = 0;
        Concealed = true;
        Container = true;
        Open = true;
        Sacred = true;
        Scenery = true;
    }


    public override void Initialize()
    {
        Name = "adventurer";
    }
}

// dead you
public class Spirit : Object
{
    private const string BeyondCapability = "Even such a simple action is beyond your capabilities.";

    public Spirit()
    {
        Animate = true;
        Strength = 0;
        Concealed = true;
        Sacred = true;
        Scenery = true;
    }

    public override void Initialize()
    {
        Name = "adventurer";

        Before<Enter>(() =>
        {
            if (Location is TimberRoom || Location is AragainFalls)
            {
                return Print("You cannot enter in your condition.");
            }

            return false;
        });

        Before<Superbrief, Verbose, Brief, Restore>(() => false);
        Before<Save, Again, Restart, Quit>(() => false);
        Before<Wake, Poke, Attack, Swing>(() => Print("All such attacks are vain in your condition."));
        Before<Eat, Close, Open, Deflate>(() => Print(BeyondCapability));
        Before<Inflate, Drink, TieTo, Burn>(() => Print(BeyondCapability));
        Before<MoveWith, Touch, Untie>(() => Print(BeyondCapability));
        Before<Wait>(() => Print("Might as well. You've got an eternity."));
        Before<SwitchOn>(() => Print("You need no light to guide you."));
        Before<Score>(() => Print("How can you think of your score in your condition?"));
        Before<Take>(() => Print("Your hand passes through its object."));
        Before<Inventory, Throw, Drop>(() => Print("You have no possessions."));
        Before<Diagnose>(() => Print("You are dead."));
        Before<Look>(() =>
        {
            var desc = "The room looks strange and unearthly";

            if (Location.Items.Count == 0)
            {
                desc += ".";
            }
            else
            {
                desc += " and objects appear indistinct.";
            }

            if (!Location.Light)
            {
                desc += "^Although there is no light, the room seems dimly illuminated.";
            }

            return Print(desc);
        });

        Before<Pray>(() =>
        {
            if (Location.Is<Altar>())
            {
                var (brass_lantern, troll, troll_room, adventurer) = Get<BrassLantern, Troll, TrollRoom, Adventurer>();

                if (troll_room.Has(troll))
                {
                    Flags.Troll = false;
                }

                brass_lantern.Concealed = false;

                Flags.AlwaysLit = false;
                Flags.Dead = false;
                Player.Set(adventurer);

                Print("From the distance the sound of a lone trumpet is heard. The room becomes very bright and you feel " +
                    "disembodied. In a moment, the brightness fades and you find yourself rising as if from a long sleep, " +
                    "deep in the woods. In the distance you can faintly hear a songbird and the sounds of the forest.");

                return GoTo<Forest1>();
            }

            return Print("Your prayers are not heard.");
        });

        Before(() =>
        {
            if (Verb is Direction)
            {
                return false;
            }

            return Print("You can't even do that.");
        });
    }
}