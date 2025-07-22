using System.Diagnostics;

namespace Zork1.Library.Parsing;

public class Unresolved
{
    public int Count => Objects.Count;
    public string Token { get; }
    public List<Object> Objects { get; set; }
    public Unresolved(string token, List<Object> objects)
    {
        Token = token;
        Objects = objects;
    }
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record Frame
{
    public Sub Sub { get; set; }

    public string Verb { get; set; }
    public string Prep { get; set; }
    public string VerbRoot { get; set; } // Verb might be verb + adverb e.g. "burn down" -> root is "burn", adverb is "down"
    public string Adverb { get; set; }
    public string VerbToken { get; set; }
    public HashSet<Object> Objects { get; set; } = [];
    public HashSet<Object> IndirectObjects { get; set; } = [];
    public List<Unresolved> UnresolvedObjects { get; set; } = [];
    public List<Unresolved> UnresolvedIndirectObjects { get; set; } = [];
    public HashSet<Object> ObjectTarget => Prep == null ? Objects : IndirectObjects;
    public List<Unresolved> UnresolvedObjectTarget => Prep == null ? UnresolvedObjects : UnresolvedIndirectObjects;
    public bool All { get; set; }
    public bool Except { get; set; }
    public HashSet<Object> ExceptObjects { get; set; } = [];
    public Dictionary<Object, byte> LocBytes { get; set; } = [];
    public bool IsError => Error != null && !Orphan;
    public string Error { get; set; }
    public bool Orphan => Verb == null || UnresolvedObjects.Count > 0 || UnresolvedIndirectObjects.Count > 0 ||
        (Required == 1 && Objects.Count == 0) || (Required == 2 && IndirectObjects.Count == 0);

    public int Required { get; set; }

    private string DebuggerDisplay
    {
        get
        {
            var verb = Prep != null ? $"{Verb} {Prep}" : $"{Verb}";
            return $"{verb} - Obj: {Objects.Count}, Ind: {IndirectObjects.Count}";
        }
    }
}
