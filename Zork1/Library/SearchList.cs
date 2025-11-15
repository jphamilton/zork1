using Zork1.Library.Things;

namespace Zork1.Library;

public static class SearchList
{
    // All will find all available objects, but containers must be open or transparent
    public static List<Object> All(Func<Object, bool> filter = null)
    {
        var root = Player.Location;
        return All(root, filter);
    }

    public static List<Object> All(Object root, Func<Object, bool> filter = null)
    {
        var result = new List<Object>();

        if (root == null)
        {
            return result;
        }

        All(root, result, filter);

        // exclude location if not vehicle
        if (!root.Vehicle)
        {
            result = [.. result.Where(x => x != root)];
        }

        return result;
    }

    private static void All(Object obj, List<Object> result, Func<Object, bool> filter = null)
    {
        TryAdd(obj, result, filter);

        // changed from && !c.Open
        if (obj is Container c && !c.CanSeeContents)
        {
            return;
        }

        if (obj.Children != null)
        {
            foreach (var child in obj.Children)
            {
                All(child, result, filter);
            }
        }
    }

    // Searches top level only, contents of containers are skipped unless Search = true
    // and contents are available (container is open or transparent)
    public static List<Object> Top(Object root, Func<Object, bool> filter = null)
    {
        var result = new List<Object>();

        if (root == null)
        {
            return result;
        }

        Top(root, result, filter);

        // Need to exclude vehicle - "take all" inside boat would otherwise take boat
        // exclude location if not vehicle
        if (!root.Vehicle)
        {
            result = [.. result.Where(x => x != root)];
        }

        return result;
    }

    private static void Top(Object obj, List<Object> result, Func<Object, bool> filter = null)
    {
        TryAdd(obj, result, filter);

        if (obj is Container c)
        {
            // changed from || !c.Open
            if (!c.Search || !c.CanSeeContents)
            {
                return;
            }
        }

        if (obj.Children != null)
        {
            foreach (var child in obj.Children)
            {
                Top(child, result, filter);
            }
        }
    }

    private static bool TryAdd(Object obj, List<Object> result, Func<Object, bool> filter = null)
    {
        if (obj.Parent?.Concealed == true && obj.Parent != Player.Instance)
        {
            return false;
        }

        if (!obj.Concealed && obj != Player.Instance)
        {
            if (filter != null)
            {
                if (filter(obj))
                {
                    result.Add(obj);
                    return true;
                }
            }
            else
            {
                result.Add(obj);
                return true;
            }
        }

        return false;
    }
}
