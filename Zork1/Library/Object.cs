using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Zork1.Library;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public abstract partial class Object
{
    [JsonIgnore]
    public string Name { get; set; }
    [JsonIgnore]
    public List<string> Adjectives { get; set; } = [];

    public abstract void Initialize();

    /// <summary>
    /// Normal every day object description
    /// </summary>
    [JsonIgnore]
    public string Description { get; set; }

    /// <summary>
    /// Used when description needs to change based on game conditions
    /// </summary>
    [JsonIgnore]
    public Func<string> Describe { get; set; }

    /// <summary>
    /// Description shown before player interacts with object
    /// </summary>
    [JsonIgnore]
    public string Initial { get; set; }

    /// <summary>
    /// This runs once per turn and provides clock-like behavior.
    /// Useful for moving things around like NPC's and draining
    /// batteries in a brass lantern
    /// </summary>
    [JsonIgnore]
    public Func<bool> Daemon { get; set; }

    public void StartDaemon()
    {
        Clock.Queue(this);
    }

    public void StopDaemon()
    {
        Clock.Interrupt(this);
    }


    [JsonIgnore]
    public Object Parent { get; set; }

    [JsonIgnore]
    public List<Object> Children { get; set; } = [];

    // excludes concealed and scenery
    public List<Object> Items => [.. Children.Where(x => !x.Concealed && x.Parent != null)];

    [JsonIgnore]
    public bool PluralName { get; set; }

    public int Capacity { get; set; }

    public bool ShowInitial => !Visited && !string.IsNullOrEmpty(Initial);

    #region English Stuff

    private readonly static List<char> _vowels = ['a', 'e', 'i', 'o', 'u'];
    private string _definiteArticle;
    private string _indefiniteArticle;

    // "the brass lantern"
    public string DName => $"{DArticle} {Name}";

    // "a brass lantern"
    public string IName => $"{IArticle} {Name}";

    [JsonIgnore]
    public string IArticle
    {
        get
        {
            if (string.IsNullOrEmpty(_indefiniteArticle) && !string.IsNullOrEmpty(Name))
            {
                var startsWithVowel = _vowels.Contains(Name.ToLower().First());
                _indefiniteArticle = startsWithVowel && !PluralName ? "an" : "a";
            }
            return _indefiniteArticle;
        }
        set { _indefiniteArticle = value; }
    }

    [JsonIgnore]
    public string DArticle
    {
        get
        {
            if (string.IsNullOrEmpty(_definiteArticle))
            {
                _definiteArticle = "the";
            }

            return _definiteArticle;
        }

        set => _definiteArticle = value;
    }

    #endregion

    #region Serialization

    public int Id { get; set; } // used internally for serialization

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            var name = GetType().FullName ?? "";
            foreach (char c in name)
            {
                hash = (hash * 31) + c;
            }
            return hash;
        }
    }

    #endregion

    private string DebuggerDisplay
    {
        get
        {
            var name = Name != null ? Name : GetType().Name;
            var loc = Parent != null ? Parent.Name : "Unknown";
            return $"{name} (at {loc})";
        }
    }

    public override string ToString() => Name;
}
