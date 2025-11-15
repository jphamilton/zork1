namespace Zork1.Scenic;

public class Tree : Object
{
    public Tree()
    {
        Climable = true;
        Scenery = true;
    }

    public override void Initialize()
    {
        Name = "tree";
        Adjectives = ["large", "storm", "tree", "branch"];
    }
}
