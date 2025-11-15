using Zork1.Library;
using Zork1.Rooms;
using Zork1.Things;

namespace Zork1.Handlers;

public class Shake : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (noun.Animate)
        {
            return Print("This seems to have no effect.");
        }

        if (!noun.Takeable)
        {
            return Print("You can't take it; thus, you can't shake it!");
        }

        // fixing discepancy in the original game - WaterFunction should be
        // called here, because it handles "all things water"
        if (noun is GlassBottle && noun.Open)
        {
            return Redirect.To<Drop>(Get<QuantityOfWater>());
        }

        if (noun is Container c)
        {
            var notEmpty = c.Items.Count > 0;

            if (c.Open)
            {
                if (notEmpty)
                {
                    Empty(c);

                    var msg = $"The contents of the {c} spills ";

                    if (!Location.DryLand)
                    {
                        msg += "out and disappears.";
                    }
                    else
                    {
                        msg += "to the ground.";
                    }

                    return Print(msg);
                }

                return Print("Shaken.");
            }

            if (notEmpty)
            {
                return Print($"It sounds like there is something inside the {c}.");
            }

            return Print($"The {noun} sounds empty.");
        }

        return Print("Shaken.");
    }

    private void Empty(Container c)
    {
        // on that off chance you're shaking an open container up in a tree...we handle that
        var dest = Location is UpATree ? Get<ForestPath>() : Location.DryLand ? Location : Get<Void>();
        var contents = c.Items.ToList();

        for (var i = 0; i < contents.Count; i++)
        {
            var child = contents[i];
            child.Visited = true;
            child.Move(dest);
        }
    }
}
