using Zork1.Handlers;
using Zork1.Library.Utilities;
using Zork1.Things;

namespace Zork1.Library;

public static class Tables
{
    public static PickOne<string> Door = new([
        "You cannot damage this door.",
        "The door is still under warranty."]
    );

    public static PickOne<string> Dummy = new([
        "Look around.",
        "Too late for that.",
        "Have your eyes checked."
    ]);

    public static PickOne<string> Hello = new([
        "Hello.",
        "Good day.",
        "Nice weather we've been having lately.",
        "Goodbye."
    ]);

    public static PickOne<string> HoHum = new([
        "doesn't seem to work.",
        "isn't notably helpful.",
        "has no effect.",
    ]);

    public static PickOne<string> Hop = new([
        "Very good. Now you can go to the second grade.",
        "Are you enjoying yourself?",
        "Wheeeeeeeeee!!!!!",
        "Do you expect me to applaud?"
    ]);

    public static PickOne<string> NoSwim = new([
        "You can't swim in the dungeon."
    ]);

    public static PickOne<string> Yuks = new([
        "A valiant attempt.",
        "You can't be serious.",
        "An interesting idea...",
        "What a concept!"
    ]);

    public static Func<string, string> HackHack = (string str) =>
    {
        var noun = Context.Noun;
        var verb = Context.Verb;

        if (noun is IGlobalObject && (verb is Lower || verb is Raise || verb is Wield))
        {
            return $"The {noun} isn't here!";
        }

        return $"{str} {noun} {HoHum.Pick()}";
    };
}
