using Zork1.Library;
using Zork1.Things;

namespace Zork1.Handlers;

public class TieTo : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (second is Me)
        {
            return Print("You can't tie anything to yourself.");
        }

        return Print($"You can't tie the {noun} to that.");
    }
}