using Zork1.Handlers;
using Zork1.Library.Things;
using Zork1.Library.Utilities;
using Zork1.Rooms;
using Zork1.Scenic;
using Zork1.Things;

namespace Zork1.Library;
public class JigsUp : Dead
{
    public string DeathMessage { get; set; }

    public override bool Handler(Object noun, Object second)
    {
        if (Flags.Dead)
        {
            Print("^It takes a talented person to be killed while already dead. YOU are such a talent. " +
                "Unfortunately, it takes a talented person to deal with it. I am not such a talent. Sorry.");
            LeaveGame();
        }

        Print(DeathMessage);

        if (!Flags.Lucky)
        {
            Print("^Bad luck, huh?");
        }

        Score.Update(-10);

        Print(" ^   ****  You have died  **** ^^");

        if (Player.InVehicle)
        {
            player.Move(Location);
        }

        if (State.Deaths > 2)
        {
            Print("You clearly are a suicidal maniac. We don't allow psychotics in the cave, since they may " +
                "harm other adventurers. Your remains will be installed in the Land of the Living Dead, where " +
                "your fellow adventurers may gloat over them.");

            return LeaveGame();
        }

        State.Deaths++;

        player.Move(Location);

        var (altar, trap_door) = Get<Altar, TrapDoor>();

        if (altar.Visited)
        {
            Print("As you take your last breath, you feel relieved of your burdens. The feeling passes as you " +
                "find yourself before the gates of Hell, where the spirits jeer at you and deny you entry. Your " +
                "senses are disturbed. The objects in the dungeon appear indistinct, bleached of color, even unreal.");
            
            Flags.Dead = true;
            Flags.Troll = true;
            Flags.AlwaysLit = true;
            Player.Set(Get<Spirit>());
            GoTo<EntranceToHades>();
        }
        else
        {
            Print("Now, let's take a look here... Well, you probably deserve another chance. I can't quite fix you up " +
                "completely, but you can't have everything.");
            GoTo<Forest1>();
        }

        trap_door.Visited = false;

        RandomizeObjects();
        
        KillInterrupts();
        
        return true;
    }

    private bool LeaveGame()
    {
        Score.PrintScore(false);
        Flags.Done = true;
        return true;
    }

    private void RandomizeObjects()
    {
        var (brass_lantern, gold_coffin, sword) = Get<BrassLantern, GoldCoffin, Sword>();

        if (Player.Has(brass_lantern))
        {
            brass_lantern.Move<LivingRoom>();
            brass_lantern.On = false;
            brass_lantern.Light = false;
        }

        if (Player.Has(gold_coffin))
        {
            gold_coffin.Move<EgyptianRoom>();
        }

        sword.VillainNear = 0;

        var dark_rooms = new PickOne<Object>([.. Objects.All.Where(x => x is Room && x.DryLand && !x.Light)]);
        var above_ground = new PickOne<Object>([.. Objects.All.Where(x => x is Room && x is AboveGround)]);
        
        foreach (var obj in Player.Children)
        {
            if (obj.TrophyValue > 0)
            {
                obj.Move(dark_rooms.Pick());
            }
            else
            {
                obj.Move(above_ground.Pick());
            }
        }
    }

    private void KillInterrupts()
    {
        var (sword, pair_of_candles, cyclops, brass_lantern, match_book) = Get<Sword, PairOfCandles, Cyclops, BrassLantern, Matchbook>();
        cyclops.StopDaemon();
        sword.StopDaemon();
        pair_of_candles.StopDaemon();
        match_book.StopDaemon();
        brass_lantern.StopDaemon();
        match_book.Flame = false;
        pair_of_candles.Flame = false;
    }
}