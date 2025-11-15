using Zork1.Library;

namespace Zork1.Scenic;

public class AltarScenery : Supporter
{
    public AltarScenery()
    {
        Capacity = 50;
    }

    public override void Initialize()
    {
        Name = "altar";
        Adjectives = ["altar"];

    }
}
