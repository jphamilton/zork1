using Zork1.Library.Things;
using Zork1.Things;

namespace Zork1.Library;

public static class Query
{
    public static bool Light(Object root = null)
    {
        if (Flags.AlwaysLit) // && player == actor
        {
            return true;
        }

        if (root == null)
        {
            root = Player.Location;
        }

        if (root.Vehicle)
        {
            root = root.Parent;
        }

        return HasLight(root);
    }

    private static bool HasLight(Object current)
    {
        if (current.Light)
        {
            return true;
        }

        if (current.Container && (!current.Open && !current.Transparent))
        {
            return false;
        }

        bool lit = false;

        if (current.Children != null)
        {
            foreach (var child in current.Children)
            {
                lit = lit || HasLight(child);
            }
        }

        return lit;
    }

    public static int Weight(Object root)
    {
        int weight = 0;
        GetWeight(root, ref weight);
        return weight;
    }

    private static void GetWeight(Object obj, ref int weight)
    {
        if (obj.Parent == Player.Instance && obj.Clothing && obj.Worn)
        {
            // I guess it's taking up less space if you're wearing it
            weight++;
        }
        else
        {
            weight += obj.Size;
        }

        if (obj.Children != null)
        {
            foreach (var child in obj.Children)
            {
                GetWeight(child, ref weight);
            }
        }
    }

    public static int CCount(Object obj)
    {
        int count = 0;
        GetCount(obj, ref count);
        return count;
    }

    private static void GetCount(Object obj, ref int count)
    {
        if (obj.Parent != Player.Instance || !obj.Clothing || !obj.Worn)
        {
            count++;
        }

        if (obj.Children != null)
        {
            foreach (var child in obj.Children)
            {
                GetCount(child, ref count);
            }
        }
    }
}
