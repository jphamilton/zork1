using System.Reflection;

namespace Zork1.Library;

public static class Routines
{
    private static List<Sub> _subs;

    public static void Load()
    {
        _subs = [];

        static void add(IEnumerable<Type> list)
        {
            foreach (var type in list.Where(t => t.IsSubclassOf(typeof(Sub)) && !t.IsAbstract).ToList())
            {
                var sub = Activator.CreateInstance(type) as Sub;
                _subs.Add(sub);
            }
        }

        // add library routines
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        add(types);
    }

    public static List<Sub> List
    {
        get { return _subs; }
    }

    public static T Get<T>() where T : Sub
    {
        return (T)List.FirstOrDefault(x => x.GetType() == typeof(T));
    }

    public static JigsUp GetJigsUp()
    {
        return (JigsUp)List.SingleOrDefault(x => x is JigsUp);
    }
}
