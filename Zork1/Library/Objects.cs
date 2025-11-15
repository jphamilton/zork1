using System.Reflection;
using Zork1.Library.Extensions;

namespace Zork1.Library;

public static class Objects
{
    // all game objects
    private static List<Object> _objects = [];
    private static Dictionary<Type, Object> _lookup = [];

    public static void Load()
    {
        _objects = [];
        _lookup = [];

        int id = 0;

        void Add(IList<Type> types)
        {
            foreach (var type in types)
            {
                if (Activator.CreateInstance(type) is Object obj)
                {
                    obj.Parent = null;
                    obj.Id = ++id;

                    if (!All.Contains(obj))
                    {
                        _objects.Add(obj);
                    }

                    _lookup.TryAdd(type, obj);
                }
            }
        }

        Assembly ax = typeof(Objects).Assembly;

        Add(ax.GetObjectTypes());
    }

    public static List<Object> All => _objects;

    public static T Get<T>() where T : Object
    {
        return (T)_lookup[typeof(T)];
    }

    public static (A, B) Get<A, B>() where A : Object where B : Object => (Get<A>(), Get<B>());

    public static (A, B, C) Get<A, B, C>()
        where A : Object
        where B : Object
        where C : Object => (Get<A>(), Get<B>(), Get<C>());

    public static (A, B, C, D) Get<A, B, C, D>()
        where A : Object
        where B : Object
        where C : Object
        where D : Object => (Get<A>(), Get<B>(), Get<C>(), Get<D>());

    public static (A, B, C, D, E) Get<A, B, C, D, E>()
        where A : Object
        where B : Object
        where C : Object
        where D : Object
        where E : Object => (Get<A>(), Get<B>(), Get<C>(), Get<D>(), Get<E>());

    // for unit tests only
    public static void Add(Object obj)
    {
        All.Add(obj);
        _lookup.Add(obj.GetType(), obj);
    }
}
