using Zork1.Things;

namespace Zork1.Library.Parsing;

public static class Lexer
{
    // verbs that support syntax like "give cyclops food"
    private static readonly List<string> _swap = ["give"];

    // verbs that support syntax like "throw lamp overboard"
    private static readonly List<string> _split = ["throw"];

    /// <summary>
    /// Lexer gonna lex
    /// </summary>
    /// <param name="command">Command entered (e.g. "open mailbox")</param>
    /// <param name="previous">Previous Parsed, used for handling partial responses</param>
    public static Frame Tokenize(string command, Frame previous = null)
    {
        var frame = new Frame();

        var tokens = command.Split(' ').ToList();

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            var original = token;

            if (IgnoredWords.Contains(token))
            {
                continue;
            }

            token = ReplacedWords.Get(token);

            if (token == "all")
            {
                if (!TryHandleAll(frame))
                {
                    break;
                }

                continue;
            }

            if (token == "except")
            {
                frame.Except = true;
                continue;
            }

            // special handling for "go"
            // "go in" and "go out" become enter and exit
            // "go in window" becomes "enter window"
            // "go up/down" becomes "climb up/down"
            if (frame.Verb == "go")
            {
                HandleGo(frame, tokens, token, i);
                continue;
            }
            else if (TryPreposition(frame, previous, token))
            {
                continue;
            }

            if (Dictionary.Verbs.Contains(token))
            {
                if (frame.Verb != null)
                {
                    if (Dictionary.Directions.Contains(token))
                    {
                        frame.Verb = token;
                        previous = null;
                        continue;
                    }
                    else if (Dictionary.Objects.Contains(token))
                    {
                        if (HandleObject(frame, tokens, token, i))
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        frame.Error = $"You used the word \"{original ?? token}\" in a way that I don't understand.";
                        break;
                    }
                }

                frame.Verb = token;
                previous = null;

                // verbs for many syntaxes get narrowed to a single word ("carry/catch/get/grab/hold/remove" -> "take")
                // but we want the parser to speak to us in the language we used:
                // > grab
                // What do you want to grab?
                frame.VerbToken = original ?? token;
                continue;
            }

            // numbers
            if (int.TryParse(token, out int value))
            {
                var number = Objects.Get<Number>();
                number.Value = value;

                if (frame.Objects.Count > 0)
                {
                    frame.IndirectObjects.Add(number);
                }
                else
                {
                    frame.Objects.Add(number);
                }

                continue;
            }

            if (Dictionary.Objects.Contains(token))
            {
                if (!HandleObject(frame, tokens, token, i))
                {
                    break;
                }
            }
            else
            {
                // failed dictionary lookup
                frame.Error = Messages.DontKnowThatWord(token);
                break;
            }
        }

        if (frame.Verb == null && frame.Prep != null && Dictionary.Verbs.Contains(frame.Prep))
        {
            frame.Verb = previous?.Verb ?? frame.Prep;
        }

        frame = Orphan.Merge(frame, previous);

        if (frame.Verb == null)
        {
            // when does this get hit?
            frame.Error = Messages.NoVerbInSentence;
            return frame;
        }

        // SPECIAL CASES - where objects need to be swapped around
        // put down lamp -> put lamp down -
        //  once parser sees "down" it will think that the objects being parsed
        //  are indirect objects - this will flip them
        // also handles things like "give cyclops food" -> "give food to cyclops"
        TrySwap(frame);

        return frame;
    }

    private static bool HandleObject(Frame frame, List<string> tokens, string token, int i)
    {
        var objects = GetObject(token);
        var found = objects.Where(o => o.Adjectives.Contains(token)).ToList();

        if (found.Count > 1)
        {
            // try to narrow by adjectives using previously found object
            var exists = frame.ObjectTarget.Where(x => x.Adjectives.Contains(token)).ToList();
            if (exists.Count > 0)
            {
                // it was found so skip this token
                //continue; // ignore token
                return true;
            }

            // look ahead for token that might resolve the object
            for (var j = i + 1; j < tokens.Count; j++)
            {
                var next = GetObject(tokens[j]);

                var intersect = next.Intersect(found).ToList();
                if (intersect.Count > 0)
                {
                    token = tokens[j];
                    found = intersect;
                }
                else
                {
                    break;
                }

                if (found.Count == 1)
                {
                    break;
                }
            }
        }

        if (found.Count > 1)
        {
            frame.UnresolvedObjectTarget.Add(new Unresolved(token, found));
            return true;
        }

        if (found.Count == 1)
        {
            if (frame.Except)
            {
                if (!frame.ExceptObjects.Contains(found[0]))
                {
                    frame.ExceptObjects.Add(found[0]);
                }
            }
            else if (found[0] is It)
            {
                // > open it
                if (Last.Noun != null && !frame.ObjectTarget.Contains(Last.Noun))
                {
                    frame.ObjectTarget.Add(Last.Noun);
                }
            }
            else
            {
                // "take rusty dead lamp" -> "take lamp"
                if (!frame.ObjectTarget.Contains(found[0]))
                {
                    frame.ObjectTarget.Add(found[0]);
                }
            }
        }
        else
        {
            // Zork seems to ignore objects in the "except" clause that are not present
            if (!frame.Except)
            {
                // TODO: May add "missing object" place holder instead
                // Zork will execute commands with objects that are available
                // and display:
                // no objects -> The objects you mentioned aren't here
                // some missing -> The other object(s) you mentioned aren't here
                frame.Error = Messages.CantSeeThatHere(token);
                return false;
            }
        }

        return true;
    }

    private static bool TryHandleAll(Frame frame)
    {
        if (frame.Objects.Count > 0 || !string.IsNullOrEmpty(frame.Prep) || frame.IndirectObjects.Count > 0)
        {
            frame.Error = Messages.DontRecognizeSentence;
            return false;
        }

        frame.All = true;

        return true;
    }

    private static void HandleGo(Frame frame, List<string> tokens, string token, int i)
    {
        if (i == tokens.Count - 1)
        {
            frame.Verb = token;
        }
        else
        {
            if (token == "in" || token == "out")
            {
                frame.Verb = token == "in" ? "enter" : "exit";
            }
            else if (token == "up" || token == "down")
            {
                frame.Verb = "climb";
                frame.Prep = token;
            }
            else
            {
                frame.Verb = token;
            }
        }
    }

    private static bool TryPreposition(Frame frame, Frame previous, string token)
    {
        if (Dictionary.Prepositions.Contains(token))
        {
            var verb = frame.Verb ?? previous?.Verb;

            if (Dictionary.Verbs.Contains($"{verb} {token}"))
            {
                // not a prep, but an adverrrrrb!
                frame.Verb = $"{verb} {token}";
                frame.VerbToken = $"{frame.VerbToken} {token}";
                frame.VerbRoot = verb!;
                frame.Adverb = token;
                // we do not set frame.Prep here because this command might
                // have a structure like "burn down house with torch"
                // so setting to "down" is wrong bc "with" will be the actual prep
                return true;
            }

            frame.Prep = token;
            return true;
        }

        return false;
    }

    private static void TrySwap(Frame frame)
    {
        if (frame.Objects.Count == 0 && frame.UnresolvedObjects.Count == 0 && frame.IndirectObjects.Count > 0)
        {
            frame.Objects = [.. frame.IndirectObjects];
            frame.IndirectObjects = [];
        }

        if (frame.Objects.Count == 0 && frame.UnresolvedObjects.Count == 0 && frame.UnresolvedIndirectObjects.Count > 0)
        {
            frame.UnresolvedObjects = [.. frame.UnresolvedIndirectObjects];
            frame.UnresolvedIndirectObjects = [];
        }

        // SPECIAL CASE (SWAP)
        // syntax like "give cyclops food"
        if (IsSwap(frame))
        {
            var objects = frame.Objects.ToList();
            frame.IndirectObjects = [objects[0]];
            objects.RemoveAt(0);
            frame.Objects = [.. objects];
        }

        // SPECIAL CASE (SPLIT)
        // syntax like "throw lamp overboard"
        if (IsSplit(frame))
        {
            var objects = frame.Objects.ToList();
            frame.IndirectObjects = [objects[1]];
            objects.RemoveAt(1);
            frame.Objects = [.. objects];
        }
    }

    public static bool IsSwap(Frame frame)
    {
        return _swap.Contains(frame.Verb) && frame.Prep == null && frame.Objects.Count > 1;
    }

    public static bool IsSplit(Frame frame)
    {
        return _split.Contains(frame.Verb) && frame.Prep == null && frame.Objects.Count > 1;
    }

    private static List<Object> GetObject(string token)
    {
        // search all objects at players location
        var objects = SearchList.All(x => x.Adjectives.Contains(token));
        return objects.Count > 0 ? objects : [.. Objects.All.Where(x => x is IGlobalObject && x.Adjectives.Contains(token))];
    }
}
