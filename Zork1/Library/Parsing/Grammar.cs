using System.Diagnostics;

namespace Zork1.Library.Parsing;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Grammar
{
    public List<string> Verbs { get; set; } = [];
    public List<string> Prepositions { get; set; } = [];
    public required Sub Handler { get; set; }
    public required ObjSpec Object { get; set; }
    public required ObjSpec IndirectObject { get; set; }
    public int Required { get; set; }

    private string DebuggerDisplay
    {
        get
        {
            var verb = $"{Verbs[0]}";
            var preps = string.Join("/", Prepositions);
            verb = $"{verb} {preps}".Trim();
            return $"{verb} - {Handler.GetType().Name}";
        }
    }
}
