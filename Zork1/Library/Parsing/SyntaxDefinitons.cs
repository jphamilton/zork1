namespace Zork1.Library.Parsing;

public static class LocBit
{
    // Possession-related flags
    public const byte NOT_USED = 0; // not used
    public const byte HAVE = 1;     // must be in player's possession
    public const byte MANY = 2;     // multiple objects allowed
    public const byte TAKE = 3;     // try to take if necessary
    // Location-related flags
    public const byte INROOM = 4;   // not at top-level, contained in another object in the room
    public const byte ONGROUND = 5; // at top-level of room and not in container
    public const byte CARRIED = 6;  // not at player level, but inside container
    public const byte HELD = 7;     // top level of player
}

public class ObjSpec
{
    public Func<Object, bool> Gwim { get; set; }
    public byte LocByte { get; set; }
}

public static class Handlers
{
    private static List<Sub> _subs = [];

    public static void Clear() => _subs = [];

    public static void Add(Sub sub)
    {
        _subs.Add(sub);
    }

    public static List<Sub> All => _subs;
}

public class SyntaxDefinitons
{
    public static List<Grammar> Grammars { get; private set; } = [];

    public class Handler
    {

        private readonly SyntaxDefinitons _syntax;
        private readonly List<string> _verbs;
        private List<string> _adverbs = [];
        private List<string> _prepositions = [];
        private ObjSpec _noun;
        private ObjSpec _second;

        public Handler(SyntaxDefinitons syntax, params List<string> names)
        {
            _syntax = syntax;
            _verbs = names;
        }

        private void CreateGrammar<T>(int required) where T : Sub, new()
        {
            var withAdverbs = new List<string>();

            // Add to Dictionary
            foreach (var verb in _verbs.ToList())
            {
                Dictionary.AddVerb(verb);

                foreach (var adverb in _adverbs)
                {
                    var a = $"{verb} {adverb}";

                    if (!withAdverbs.Contains(a))
                    {
                        // verb def below needs to get replaced
                        //_verbs.Add(a);
                        withAdverbs.Add(a);
                    }

                    Dictionary.AddVerb(a);
                }
            }

            foreach (var prep in _prepositions)
            {
                Dictionary.AddPreposition(prep);
            }

            var verbs = withAdverbs.Count > 0 ? withAdverbs : _verbs;

            var grammar = new Grammar
            {
                Verbs = verbs,
                Prepositions = [.. _prepositions],
                Object = _noun,
                IndirectObject = _second,
                Handler = CreateSub<T>(),
                Required = required
            };

            Grammars.Add(grammar);

            // must clear prepositions here!
            _adverbs = [];
            _prepositions = [];
        }

        private Sub CreateSub<T>() where T : Sub, new()
        {
            Sub sub = Handlers.All.SingleOrDefault(x => x.GetType() == typeof(T));

            if (sub == null)
            {
                sub = new T();
                Handlers.Add(sub);
            }

            return sub;
        }

        public Handler Add<T>() where T : Sub, new()
        {
            CreateGrammar<T>(0);
            return this;
        }

        public Handler Add<T>(string prepositions) where T : Sub, new()
        {
            _prepositions = [.. prepositions.Split('/')];
            CreateGrammar<T>(0);
            return this;
        }

        public Handler Add<T>(ObjSpec noun) where T : Sub, new()
        {
            _noun = noun;
            CreateGrammar<T>(1);
            return this;
        }

        public Handler Add<T>(ObjSpec noun, ObjSpec second) where T : Sub, new()
        {
            _noun = noun;
            _second = second;
            CreateGrammar<T>(2);
            return this;
        }

        public Handler Add<T>(ObjSpec noun, string prepositions, ObjSpec second) where T : Sub, new()
        {
            _noun = noun;
            _second = second;
            _prepositions = [.. prepositions.Split('/')];
            CreateGrammar<T>(2);
            return this;
        }

        public Handler Add<T>(string adverb, ObjSpec noun, string prepositions, ObjSpec second) where T : Sub, new()
        {
            //if (_verbs.Contains("turn") && adverb == "on")
            //{
            //    Debugger.Break();
            //}

            _noun = noun;
            _second = second;
            _prepositions = [.. prepositions.Split('/')];

            if (!_adverbs.Contains(adverb))
            {
                _adverbs.Add(adverb);
            }

            CreateGrammar<T>(2);
            return this;
        }

        public Handler Add<T>(string prepositions, ObjSpec noun) where T : Sub, new()
        {
            //if (_verbs.Contains("turn") && prepositions == "on")
            //{
            //    Debugger.Break();
            //}

            _noun = noun;
            _prepositions = [.. prepositions.Split('/')];
            CreateGrammar<T>(1);
            return this;
        }

        public SyntaxDefinitons End()
        {
            return _syntax;
        }

        public Handler Verb(string verbs)
        {
            return new Handler(_syntax, [.. verbs.Split('/')]);
        }
    }

    private SyntaxDefinitons()
    {

    }

    public static SyntaxDefinitons Begin()
    {
        Handlers.Clear();

        Grammars = [];

        return new SyntaxDefinitons();
    }

    public Handler Verb(string verbs)
    {
        return new Handler(this, [.. verbs.Split('/')]);
    }
}