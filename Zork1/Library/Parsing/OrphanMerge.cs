using Zork1.Library.Extensions;

namespace Zork1.Library.Parsing;

public static class Orphan
{
    // I feel like this is one of those things that a
    // good programmer could do in 4 lines of code.
    public static Frame Merge(Frame frame, Frame previous)
    {
        if (frame.Error == null && previous?.Orphan == true)
        {
            frame.Verb ??= previous.Verb;

            var unresolvedObjects = frame.UnresolvedObjectTarget;

            if (frame.Prep == null && frame.UnresolvedObjectTarget.Count == 0 && previous.UnresolvedObjectTarget.Count == 0)
            {
                previous.Verb = frame.Verb;
                previous.ObjectTarget.AddRange(frame.ObjectTarget);
                previous.Prep ??= frame.Prep!;
                previous.Error = null;
                frame = previous;
            }

            foreach (var unresolved in previous.UnresolvedObjectTarget.ToList())
            {
                foreach (var obj in unresolved.Objects)
                {
                    if (!frame.Objects.Contains(obj))
                    {
                        continue;
                    }

                    var found = frame.Objects.SingleOrDefault(x => x == obj);

                    if (found == null)
                    {
                        continue;
                    }

                    foreach (var pur in previous.UnresolvedObjectTarget.ToList())
                    {
                        var resolved = found.Adjectives.Contains(pur.Token) ? found : null;
                        if (resolved != null)
                        {
                            previous.Error = null;
                            previous.UnresolvedObjectTarget.Remove(pur);
                            previous.ObjectTarget.AddRange(frame.Objects);
                        }
                    }

                    frame = previous;
                }
            }
        }

        if (frame.Error != null || frame.Orphan)
        {
            // if previous frame is also an orphan, copy everything forward
            if (previous?.Orphan == true)
            {
                frame.Required = previous.Required;
                frame.Verb = previous.Verb;
                frame.VerbToken = previous.VerbToken;
                frame.Objects.AddRange(previous.Objects);
                frame.IndirectObjects.AddRange(previous.IndirectObjects);
                frame.UnresolvedObjects.AddRange(previous.UnresolvedObjects);
                frame.UnresolvedIndirectObjects.AddRange(previous.UnresolvedIndirectObjects);

                if (frame.Orphan && frame.UnresolvedObjects.Count == 0 && frame.UnresolvedIndirectObjects.Count == 0)
                {
                    // clear so that Orphan evaluates to false, because object is not found
                    frame.Required = 0;
                }
            }
        }

        return frame;
    }
}
