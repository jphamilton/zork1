using Zork1.Library;
using Zork1.Things;

namespace Zork1.Handlers;

public class Throw : Sub
{
    public Throw()
    {
        PreSub = PreThrow;
    }

    // https://github.com/the-infocom-files/zork1/issues/52
    // https://github.com/the-infocom-files/zork1/issues/53
    // https://github.com/the-infocom-files/zork1/issues/54
    public static bool PreThrow(Object noun, Object second)
    {
        // this will let you throw yourself off cliffs, but not at creatures or other things
        if (noun is Me && (second == null || second.Scenery))
        {
            return false;
        }

        if (noun is IGlobalObject)
        {
            return Print(Tables.Yuks.Pick());
        }

        //if (!Player.Has(noun))
        //{
        //    return Print(Messages.DontHaveThat);
        //}

        return false;
    }

    public override bool Handler(Object noun, Object second)
    {
        if (!Drop.PerformDrop(noun, Location))
        {
            return false;
        }

        if (second is Me)
        {
            return JigsUp($"A terrific throw! The {noun} hits you squarely in the head. Normally, this wouldn't " +
                $"do much damage, but by incredible mischance, you fall over backwards trying to duck, " +
                $"and break your neck, justice being swift and merciful in the Great Underground Empire.");
        }

        if (second != null && second.Animate)
        {
            return Print($"The {second} ducks as the {noun} flies by and crashes to the ground.");
        }

        return Print("Thrown.");
    }
}