namespace Zork1.Library.Utilities;

public class PickOne<T>(List<T> messages)
{
    private List<T> _picked = [];
    private List<T> _unpicked = messages;

    public T Pick()
    {
        var index = Random.Number(_unpicked.Count);
        var result = _unpicked[index];

        _unpicked.Remove(result);
        _picked.Add(result);

        if (_unpicked.Count == 0)
        {
            _unpicked = [.. _picked];
            _picked.Clear();
        }

        return result;
    }

    // for testing
    public List<T> List => [.. _picked, .. _unpicked];
}
