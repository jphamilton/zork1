using System.Text;
using Zork1.Library.Extensions;

namespace Zork1.Library;

public static class Describer
{
    public static string Object(Supporter supporter)
    {
        if (supporter.Concealed || !supporter.CanSeeContents)
        {
            return null;
        }

        var contents = supporter.Items.ToList();
        var initial = contents.Where(x => x.ShowInitial).ToList();
        var containers = contents.Where(x => x is Container).ToList();

        List<string> results = [];

        if (contents.Count > 0)
        {
            foreach (var item in initial)
            {
                results.Add(item.Initial);
                contents.Remove(item);
            }

            if (contents.Count > 0)
            {
                List<string> list = [];

                foreach (var item in contents)
                {
                    if (item is Container c)
                    {
                        list.Add(Object(c));
                    }
                    else
                    {
                        list.Add(item.IName);
                    }
                }

                if (list.Count > 0)
                {
                    results.Add($"On the {supporter} is {list.Join("and").Trim()}.");
                }
            }
        }

        if (!supporter.Scenery)
        {
            results.Add($"There is a {supporter} here.");
        }

        return string.Join(Environment.NewLine, results);
    }

    public static string Object(Container container)
    {
        if (container.Concealed || container.Scenery)
        {
            return null;
        }

        if (container.CanSeeContents)
        {
            var list = GetContents(container);
            return list.Count > 0 ? $"a {container} (which contains {list.Join("and")})" : $"a {container} (which is empty)";
        }

        return $"a {container}";
    }

    public static List<string> GetContents(HasContents container)
    {
        List<string> list = [];

        if (container.Concealed || !container.CanSeeContents)
        {
            return list;
        }

        foreach (var obj in container.Items)
        {
            if (obj.Concealed)
            {
                continue;
            }

            // This leads to something like this:
            // a jewel-encrusted egg (which contains There is a golden clockwork canary
            // nestled in the egg.It has ruby eyes and a silver beak.Through a crystal window
            // below its left wing you can see intricate machinery inside.It appears to have
            // wound down.)
            //
            //if (obj.ShowInitial)
            //{
            //    list.Add(obj.Initial);
            //    continue;
            //}

            if (obj is Container c)
            {
                list.Add(Object(c));
            }
            else
            {
                list.Add(obj.IName);
            }
        }

        return list;
    }

    public static string DisplayList(Object root)
    {
        return DisplayList(root.Children);
    }

    public static string DisplayList(List<Object> objects)
    {
        var sb = new StringBuilder();

        var containers = new List<Object>();

        foreach (var obj in objects.Where(x => !x.Concealed).ToList())
        {
            if (obj is Container container)
            {
                containers.Add(container);

                if (container.CanSeeContents)
                {
                    sb.AppendLine(DisplayListContainer(container, 1));
                }
                else
                {
                    sb.AppendLine($"  {DisplayListObject(container)}");
                }
            }

            SetLast.Object(obj);
        }

        foreach (var obj in objects.Where(x => !x.Concealed && !containers.Contains(x)).OrderBy(x => x.Description))
        {
            sb.AppendLine($"  {DisplayListObject(obj)}");
        }

        var desc = sb.ToString();
        return desc.Length > 0 ? desc[..desc.LastIndexOf(Environment.NewLine)] : null;
    }

    private static string DisplayListObject(Object obj)
    {
        var asides = new List<string>();

        if (obj.Light)
        {
            asides.Add("providing light");
        }

        if (obj.Clothing && obj.Worn)
        {
            asides.Add("being worn");
        }

        if (obj is Container container && container.Openable)
        {
            string clause = asides.Count == 0 ? "which is " : "";

            if (container.Open)
            {
                asides.Add(container.Items.Count > 0 ? $"{clause}open" : $"{clause}open but empty");
            }
            else
            {
                asides.Add($"{clause}closed");
            }
        }

        var aside = asides.Count > 0 ? $" ({string.Join(" and ", asides)})" : "";

        return obj.IName + aside;
    }

    private static string DisplayListContainer(Container container, int level)
    {
        StringBuilder sb = new();
        sb.Append(new string(' ', level * 2));
        sb.Append(Object(container));
        return sb.ToString();
    }
}
