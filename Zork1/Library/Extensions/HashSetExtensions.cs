namespace Zork1.Library.Extensions;

public static class HashSetExtensions
{
    public static void AddRange<T>(this HashSet<T> objects, IEnumerable<T> adding) where T : class
    {
        foreach (var obj in adding)
        {
            objects.Add(obj);
        }
    }
}
