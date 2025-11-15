using System.Text;
using Zork1.Handlers;
using Zork1.Library.Extensions;
using Zork1.Library.Things;
using Zork1.Things;

namespace Zork1.Library.Parsing;

public static class TakeCheck
{
    public static bool Check(Frame frame)
    {
        foreach (var obj in frame.Objects)
        {
            if (!ITakeCheck(frame, obj))
            {
                return false;
            }
        }

        foreach (var obj in frame.IndirectObjects)
        {
            if (!ITakeCheck(frame, obj))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ITakeCheck(Frame frame, Object obj)
    {
        if (!frame.LocBytes.TryGetValue(obj, out var locByte))
        {
            return false;
        }

        bool taken = false;

        if (!locByte.Has(LocBit.HAVE) && !locByte.Has(LocBit.TAKE))
        {
            return true;
        }

        if (obj is It)
        {
            obj = Last.Noun;
        }

        if (obj.In(Player.Instance) || obj is PairOfHands)
        {
            return true;
        }

        if (obj.TryTake)
        {
            taken = true;
        }
        else if (Player.Instance is not Adventurer)
        {
            taken = false;
        }
        else if (obj is Me)
        {
            return true;
        }
        else if (locByte.Has(LocBit.TAKE) && ITake(obj, false))
        {
            taken = false;
        }
        else
        {
            taken = true;
        }

        if (taken && locByte.Has(LocBit.HAVE))
        {
            if (obj is NotHere)
            {
                Output.Print(Messages.DontHaveThat);
                return false;
            }

            frame.Error = $"You don't have the {obj}.";
            return false;
        }

        if (taken || Player.Instance is not Adventurer)
        {
            return true;
        }

        Output.Print("(Taken)");

        return true;
    }

    // this is used all over the game but we will leave here for now
    public static bool ITake(Object obj, bool vb = true)
    {
        if (Flags.Dead)
        {
            if (!vb)
            {
                return false;
            }

            Output.Print("Your hand passes through its object.");
            return false;
        }

        if (!obj.Takeable)
        {
            if (!vb)
            {
                return false;
            }

            Output.Print(Tables.Yuks.Pick());
            return false;
        }

        if (/* Zork2_deletion() || */ obj.Parent is Container c && !c.Open)
        {
            return false;
        }

        if (!Player.Has(obj.Parent))
        {
            var weight = Query.Weight(obj);
            if (weight + Query.Weight(Player.Instance) > State.LoadMax)
            {
                if (vb)
                {
                    var sb = new StringBuilder("Your load is too heavy");
                    if (State.LoadMax < State.LoadAllowed)
                    {
                        sb.Append(", especially in light of your condition.");
                    }
                    sb.Append('.');
                    Output.Print(sb.ToString());
                }

                return false;
            }
        }

        if (Context.Verb is Take)
        {
            var numItems = Query.CCount(Player.Instance);
            if (numItems > State.MaximumHeld)
            {
                var weight = numItems * State.MaxHeldMult;
                if (weight > Random.Between(1, 100))
                {
                    Output.Print("You're holding too many things already!");
                    return false;
                }
            }
        }

        // I think this is done (instead of running a Take command) is
        // so implicit taking does not kill you accidentally :)
        Player.Add(obj);

        Score.ScoreObject(obj);

        return true;
    }
}