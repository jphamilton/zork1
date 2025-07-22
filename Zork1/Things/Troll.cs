using System.Text;
using Zork1.Handlers;
using Zork1.Library.Things;

namespace Zork1.Things;

public class Troll : Villain
{
    private const string NastyTroll = "A nasty-looking troll, brandishing a bloody axe, blocks all passages out of the room.";
    private BloodyAxe bloody_axe;
    private Troll troll;

    public Troll()
    {
        Animate = true;
        Open = true;
        Strength = 2;
        TryTake = true;
    }

    public override void Initialize()
    {
        Name = "troll";
        Adjectives = ["troll", "nasty"];
        Description = NastyTroll;
        Describe = () => Description;
        bloody_axe = IsHere<BloodyAxe>();
        troll = this;
        Before<Tell>(() => Print("The troll isn't much of a conversationalist."));
        Before<Examine>(() => Print(Description));
        Before<Listen>(() => Print("Every so often the troll says something, probably uncomplimentary, in his guttural tongue."));
        Before<Hello>(() =>
        {
            if (!Flags.Troll)
            {
                return Print("Unfortunately, the troll can't hear you.");
            }

            return false;
        });
        Before<Move, Take>(() => Print("The troll spits in your face, grunting ~Better luck next time~ in a rather barbarous accent."));
        Before<Poke>(() => Print("The troll laughs at your puny gesture."));
        Before<Give, Throw>(() =>
        {
            if (Noun == null || Second != troll)
            {
                return false;
            }

            CheckStrength();

            if (Noun == bloody_axe && Player.Has(bloody_axe))
            {
                troll.Fight = true;
                bloody_axe.Move(troll);
                return Print("The troll scratches his head in confusion, then takes the axe.");
            }

            if (Noun == troll || Noun == bloody_axe)
            {
                return Print($"You would have to get the {Noun} first, and that seems unlikely.");
            }

            StringBuilder sb = new();

            if (Verb is Throw)
            {
                sb.Append($"The troll, who is remarkably coordinated, catches the {Noun}");
            }
            else
            {
                sb.Append("The troll, who is not overly proud, graciously accepts the gift");
            }

            if (20 > Random.Number(100) && (Noun is NastyKnife || Noun is Sword || Noun == bloody_axe))
            {
                Noun.Remove();
                troll.Remove();
                troll.OnKilled();
                Flags.Troll = true;
                sb.Append(" and eats it hungrily. Poor troll, he dies from an internal hemorrhage and his carcass disappears in a sinister black fog.");
                return Print(sb.ToString());
            }

            if (Noun is NastyKnife || Noun is Sword || Noun == bloody_axe)
            {
                Noun.MoveHere();
                troll.Fight = true;
                sb.Append($" and, being for the moment sated, throws it back. Fortunately, the troll has poor control, and the {Noun} falls to the floor. He does not look pleased.");
                return Print(sb.ToString());
            }

            sb.Append(" and not having the most discriminating tastes, gleefully eats it.");
            Noun.Remove();
            return Print(sb.ToString());
        });
    }

    #region Melee

    public override bool OnDisarmed()
    {
        if (troll.Has(bloody_axe))
        {
            return false;
        }

        if (Location.Has(bloody_axe) && Random.Probability(75))
        {
            bloody_axe.Scenery = true;
            bloody_axe.Weapon = false;
            bloody_axe.Move(troll);
            Description = NastyTroll;

            if (Location.Has(troll))
            {
                return true;
            }

            return Print("The troll, angered and humiliated, recovers his weapon. He appears to have an axe to grind with you.");
        }

        if (!Location.Has(troll))
        {
            return false;
        }

        troll.Description = "A pathetically babbling troll is here.";

        return Print("The troll, disarmed, cowers in terror, pleading for his life in the guttural tongue of the trolls.");
    }

    public override bool OnKilled()
    {
        if (troll.Has(bloody_axe))
        {
            bloody_axe.MoveHere();
            bloody_axe.Scenery = false;
            bloody_axe.Weapon = true;
        }

        Flags.Troll = true;
        return true;
    }

    public override bool OnKnockedOut()
    {
        troll.Fight = false;
        if (troll.Has(bloody_axe))
        {
            bloody_axe.MoveHere();
            bloody_axe.Scenery = false;
            bloody_axe.Weapon = true;
        }

        troll.Description = "An unconscious troll is sprawled on the floor. All passages out of the room are open.";
        Flags.Troll = true;
        return true;
    }

    public override bool OnWakeUp()
    {
        if (Location.Has(troll))
        {
            troll.Fight = true;
            Print("The troll stirs, quickly resuming a fighting stance.");
        }

        if (troll.Has(bloody_axe))
        {
            troll.Description = NastyTroll;
        }
        else if (Location.Has(bloody_axe))
        {
            bloody_axe.Scenery = true;
            bloody_axe.Weapon = false;
            bloody_axe.Move(troll);
            troll.Description = NastyTroll;
        }
        else
        {
            troll.Description = "A troll is here.";
        }

        Flags.Troll = false;

        return true;
    }

    public override bool OnAttacked()
    {
        if (33 <= Random.Number(100))
        {
            return false;
        }

        troll.Fight = true;

        return true;
    }

    #endregion
}

//[AxeAction ;	! 42804 / 0xa734
//    return TryTakeWeapon(bloody_axe, troll);
//];

//[TryTakeWeapon obj villain ;	! 42814 / 0xa73e
//    if (villain notin location || Verb ~= ##Take) rfalse;
//    if (obj in villain) {
//        "The ", (name) villain, " swings it out of your reach.";
//    }
//    print "The ", (name) obj, " seems white-hot. You can't hold on to it.";
//new_line;
//rtrue;
//];