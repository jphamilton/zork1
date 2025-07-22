using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Scenic;

namespace Zork1.Rooms;

public abstract class Drafty : Room
{
    protected readonly string CantFit = "You cannot fit through this passage with that load.";

    protected bool EmptyHanded()
    {
        return !player.Children.Any(x => Query.Weight(x) > 4);
    }
}

public class DraftyRoom : Drafty
{
    public DraftyRoom()
    {
        DryLand = true;
        Sacred = true;
        TakeValue = 13;
    }

    public override void Initialize()
    {
        Name = "Drafty Room";
        Description = "This is a small drafty room in which is the bottom of a long shaft. To the south is a " +
            "passageway and to the east a very narrow passage. In the shaft can be seen a heavy iron chain.";
        WithScenery<PseudoChain>();
        IsHere<Basket2>();
        OutTo(() => EmptyHanded() ? Get<TimberRoom>() : NoGo(CantFit));
        EastTo(() => EmptyHanded() ? Get<TimberRoom>() : NoGo(CantFit));
        SouthTo<MachineRoom>();
    }
}
