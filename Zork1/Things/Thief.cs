using Zork1.Handlers;
using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Rooms;

namespace Zork1.Things;

public partial class Thief : Villain
{
    public bool Engrossed { get; set; }

    public static string ThiefOutDesc = "There is a suspicious-looking individual lying unconscious on the ground.";
    public static string ThiefDesc = "There is a suspicious-looking individual, holding a bag, leaning against one wall. He is armed with a vicious-looking stiletto.";

    private Stiletto stiletto;
    private TreasureRoom treasure_room;

    public Thief()
    {
        Animate = true;
        Concealed = true;
        Container = true;
        Open = true;
        Strength = 5;
        TryTake = true;
    }

    public override void Initialize()
    {
        Name = "thief";
        Adjectives = ["thief", "robber", "criminal", "individual", "shady", "suspicious", "seedy"];
        Description = ThiefDesc;
        IsHere<LargeBag>();
        stiletto = IsHere<Stiletto>();
        treasure_room = Get<TreasureRoom>();

        Daemon = ThiefDaemon;

        Before<Take>(() => Print("Once you got him, what would you do with him?"));
        Before<Tell>(() => Print("The thief is a strong, silent type."));
        Before<Listen>(() => Print("The thief says nothing, as you have not been formally introduced."));
        Before<LookIn, Examine>(() => Print("The thief is a slippery character with beady eyes that flit back and forth. " +
            "He carries, along with an unmistakable arrogance, a large bag over his shoulder and a vicious stiletto, whose " +
            "blade is aimed menacingly in your direction. I'd watch out if I were you."));
        Before<Hello>(() =>
        {
            if (Description == ThiefOutDesc)
            {
                return Print("The thief, being temporarily incapacitated, is unable to acknowledge your greeting with his usual graciousness.");
            }

            return false;
        });

        Before<Throw>(() =>
        {
            if (Noun is NastyKnife && !Fight)
            {
                if (Random.Probability(10))
                {
                    var r = "You evidently frightened the robber, though you didn't hit him. He flees";

                    if (Items.Count > 0)
                    {
                        r += ", but the contents of his bag fall on the floor.";

                        // I don't think the thief puts stolen things in his bag
                        foreach (var obj in Items)
                        {
                            obj.Open = true;
                            obj.Concealed = false;
                            obj.MoveHere();
                        }
                    }
                    else
                    {
                        r += ".";
                    }

                    Concealed = true;

                    return Print(r);
                }

                Fight = true;
                return Print("You missed. The thief makes no attempt to take the knife, though it would be a fine addition to the collection in his bag. He does seem angered by your attempt.");
            }

            return false;
        });

        Before<Give>(() =>
        {
            if (Noun != null && Noun != this && Second == this)
            {
                if (Strength < 0)
                {
                    Strength = 0 - Strength;
                    StopDaemon();
                    RetreiveStiletto();
                    Description = ThiefDesc;
                    Print("Your proposed victim suddenly recovers consciousness.");
                }

                Noun.Move(this);

                if (Noun.TrophyValue > 0)
                {
                    Engrossed = true;
                    return Print($"The thief is taken aback by your unexpected generosity, but accepts the {Noun} and stops to admire its beauty.");
                }

                return Print($"The thief places the {Noun} in his bag and thanks you politely.");
            }

            return false;
        });
    }
    
    public bool RetreiveStiletto()
    {
        if (!Parent.Has(stiletto))
        {
            return false;
        }

        stiletto.Scenery = true;
        stiletto.Move(this);

        return true;
    }

    private void HackTreasures()
    {
        RetreiveStiletto();

        Concealed = true;

        foreach (var obj in treasure_room.Items)
        {
            obj.Concealed = false;
        }
    }

    private List<Object> ThiefDies(Room room)
    {
        var result = new List<Object>();

        var items = Children.Where(x => x is not Stiletto && x is not LargeBag).ToList();
        
        foreach (var obj in items)
        {
            if (obj.TrophyValue > 0)
            {
                obj.Move(room);

                result.Add(obj);

                if (obj is JeweledEgg egg)
                {
                    egg.Open = true;
                }
            }
        }

        return result;
    }

}

public class LargeBag : Object
{
    public LargeBag()
    {
        TryTake = true;
        Scenery = true;
    }

    public override void Initialize()
    {
        Name = "large bag";
        Adjectives = ["bag", "large", "thiefs"];

        var thief = Get<Thief>();

        Before<Take>(() =>
        {
            if (thief.Description == Thief.ThiefOutDesc)
            {
                return Print("Sadly for you, the robber collapsed on top of the large bag. Any attempt to take it would surely rouse him to consciousness.");
            }

            return Print("The bag will be taken over his dead body.");
        });

        Before<Insert>(() =>
        {
            if (Second is LargeBag)
            {
                return Print($"The {Noun} would make a fine addition to his collection, but he is unlikely to allow you near enough to deposit it.");
            }

            return false;
        });

        Before<Open, Close>(() => Print("Getting close enough would be a good trick."));

        Before<LookIn, Examine>(() => Print("The bag is closed, so it's hard to tell what, if anything, is inside."));
    }
}

public class Stiletto : Object
{
    public Stiletto()
    {
        Size = 10;
        TryTake = true;
        Scenery = true;
        Takeable = true;
        Weapon = true;
    }

    public override void Initialize()
    {
        Name = "stiletto";
        Adjectives = ["stiletto", "vicious"];
        Before<Take>(() =>
        {
            var thief = Get<Thief>();
            return thief.TryTakeWeapon(this);
        });
    }
}