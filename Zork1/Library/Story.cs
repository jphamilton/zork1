namespace Zork1.Library;

public abstract class Story
{
    private readonly SyntaxBase _syntax;

    public string Name { get; set; }
    public string Title { get; set; }

    protected abstract void Start();

    public Story(SyntaxBase syntax)
    {
        _syntax = syntax;
    }

    public void Initialize()
    {
        Dictionary.Load();

        _syntax.Load();

        Routines.Load();

        Objects.Load();
        
        // initialize rooms first
        foreach (var obj in Objects.All.Where(x => x is Room))
        {
            obj.Initialize();
            Dictionary.AddObject(obj);
        }

        // then objects
        foreach (var obj in Objects.All.Where(x => x is not Room))
        {
            obj.Initialize();
            Dictionary.AddObject(obj);
        }

        Dictionary.Sort();

        Start();
    }
}
