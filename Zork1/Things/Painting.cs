using Zork1.Handlers;

namespace Zork1.Things;

public class Painting : Object
{
    public Painting()
    {
        TrophyValue = 6;
        TakeValue = 4;
        Size = 15;
        Takeable = true;
        Flammable = true;
    }

    public override void Initialize()
    {
        Name = "painting";
        Adjectives = ["painting", "art", "canvas", "beautiful", "treasure"];
        Initial = "Fortunately, there is still one chance for you to be a vandal, for on the far wall is a painting of unparalleled beauty.";

        Before<Poke>(() =>
        {
            // #48 https://microheaven.com/InfocomBugs/zorki.shtml
            TrophyValue = 0;
            Visited = true;
            Description = "There is a worthless piece of canvas here.";
            return Print("Congratulations! Unlike the other vandals, who merely stole the artist's masterpieces, you have destroyed one.");
        });
    }
}