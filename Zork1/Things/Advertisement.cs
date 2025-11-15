namespace Zork1.Things;

public class Advertisement : Object
{
    public Advertisement()
    {
        Flammable = true;
        Readable = true;
        Takeable = true;
        Size = 2;
    }

    public override void Initialize()
    {
        Name = "leaflet";
        Adjectives = ["leaflet", "advertisement", "booklet", "mail"];
        Description = "A small leaflet is on the ground.";

        Text = "~WELCOME TO ZORK!^" +
        "ZORK is a game of adventure, danger, and low cunning. In it you^" +
        "will explore some of the most amazing territory ever seen by mortals.^" +
        "No computer should be without one!~";
    }
}
