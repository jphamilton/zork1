using Zork1.Library;
using Zork1.Library.Parsing;

namespace Zork1.Handlers;

public class Insert : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (!second.Open && (second.Door || !second.Container))
        {
            if (!second.Vehicle)
            {
                return Print("You can't do that.");
            }
        }

        if (!second.Open)
        {
            SetLast.Object(second);
            return Print($"The {second} isn't open.");
        }

        if (second == noun)
        {
            return Print("How can you do that?");
        }

        if (second.Has(noun))
        {
            return Print($"The {noun} is already in the {second}.");
        }

        var weight = Query.Weight(second);
        weight += Query.Weight(noun);
        weight -= second.Size;

        if (weight > second.Capacity)
        {
            return Print("There's no room.");
        }

        if (!noun.In(player) && noun.TryTake)
        {
            return Print($"You don't have the {noun}.");
        }

        if (!noun.In(player) && !TakeCheck.ITake(noun))
        {
            return true;
        }

        noun.Move(second);

        noun.Visited = true;

        Score.ScoreObject(noun);

        return Print("Done.");
    }
}