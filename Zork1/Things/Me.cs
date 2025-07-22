using Zork1.Handlers;
using Zork1.Library;
using Zork1.Rooms;

namespace Zork1.Things;

// self-referential object - e.g. kill me, eat me, etc.
public class Me : GlobalObject
{
    public override void Initialize()
    {
        Name = "you";
        Adjectives = ["me", "myself", "self", "cretin"];

        Before<Tell>(() => Print("Talking to yourself is said to be a sign of impending mental collapse."));

        Before<Give>(() => Redirect.To<Take>(Noun));

        Before<Make>(() => Print("Only you can do that."));

        Before<Disembark>(() => Print("You'll have to do that on your own."));

        Before<Eat>(() => Print("Auto-cannibalism is not the answer."));

        Before<Throw>(() =>
        {
            if (Noun == this)
            {
                return Print("Why don't you just walk like normal people?");
            }

            return false;
        });

        Before<Poke, Attack>(() =>
        {
            if (Second != null && Second.Weapon)
            {
                return JigsUp("If you insist.... Poof, you're dead!");
            }

            return Print("Suicide is not the answer.");
        });

        Before<Take>(() => Print("How romantic!"));

        Before<Examine>(() =>
        {
            if (Location is MirrorRoom1 || Location is MirrorRoom2)
            {
                return Print("Your image in the mirror looks tired.");
            }

            return Print("That's difficult unless your eyes are prehensile.");
        });
    }
}
