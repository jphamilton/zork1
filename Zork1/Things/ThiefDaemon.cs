using Zork1.Library;
using Zork1.Library.Functions;
using Zork1.Library.Things;
using Zork1.Rooms;

namespace Zork1.Things;

public partial class Thief : Villain
{
    private bool ThiefDaemon()
    {
        var treasure_room = Get<TreasureRoom>();
        var room = (Room)Parent;
        bool here;
        bool once = false;

        while (true)
        {
            here = !Concealed;

            if (here)
            {
                room = (Room)Parent;
            }

            if (room != treasure_room || room == Location)
            {
                if (room != Location || !room.Light || Location.Has<Troll>())
                {
                    if (room.Has(this) && !Concealed)
                    {
                        Concealed = true;
                        here = false;
                    }

                    // In original game, the thief will never rob a maze room,
                    // because maze rooms are continually set to Visited = false (to confuse the player)
                    // https://github.com/the-infocom-files/zork1/issues/61

                    if (room.Visited)
                    {
                        Rob.Run(room, this, 75);
                        StealJunk(room);
                    }

                    if (room.MazeRoom && Location.MazeRoom)
                    {
                        RobMaze(room);
                    }
                }
                else
                {
                    if (ThiefVsAdventurer(here))
                    {
                        return true;
                    }

                    if (Concealed)
                    {
                        here = false;
                    }
                }
            }
            else
            {
                if (here)
                {
                    HackTreasures();
                    here = false;
                }
                else
                {
                    ThiefDies(treasure_room);
                }
            }

            if (once)
            {
                break;
            }

            once = true;

            RetreiveStiletto();

            if (Location == treasure_room)
            {
                continue;
            }

            room = NextRoom(room);

            Debug($"Thief moves to '{room.Name}'");

            Move(room);
            Fight = false;
            Concealed = true;
            Flags.ThiefHere = false;
        }

        if (room != treasure_room)
        {
            DropJunk(room);
        }

        return true;
    }

    private void DropJunk(Room room)
    {
        bool dropflag = false;

        var junk = Children.Where(x => x.TrophyValue == 0).ToList();

        foreach (var obj in junk)
        {
            if (Random.Probability(30))
            {
                obj.Concealed = false;
                obj.Move(room);

                Debug($"Thief: DropJunk - {obj} in {room}");

                if (!dropflag && room == Location)
                {
                    Print("The robber, rummaging through his bag, dropped a few items he found valueless.");
                    dropflag = true;
                }
            }
        }
    }

    private void StealJunk(Room room)
    {
        foreach (var obj in room.Children)
        {
            if (obj.TrophyValue == 0 && obj.Takeable && !obj.Sacred && !obj.Concealed && !obj.Scenery && (obj == stiletto || Random.Probability(10)))
            {
                obj.Move(this);
                obj.Visited = true;
                obj.Concealed = true;

                Debug($"Thief: StealJunk - {obj} in {room}");

                if (obj == Get<Rope>())
                {
                    Flags.Dome = false;
                }

                if (room == Location)
                {
                    Print($"^You suddenly notice that the {obj.Name} vanished.");
                }

                return;
            }
        }
    }

    private void RobMaze(Room room)
    {
        foreach (var obj in room.Children)
        {
            if (obj.Takeable && !obj.Concealed && 40 > Random.Number(100))
            {
                Print($"You hear, off in the distance, someone saying ~My, I wonder what this fine {obj.Name} is doing here.~");

                if (!Random.Probability(60))
                {
                    return;
                }

                obj.Move(this);
                obj.Visited = true;
                obj.Concealed = true;

                return;
            }
        }
    }

    private static Room NextRoom(Room room)
    {
        var rooms = Objects.All.Where(x => x is Room && x is not Library.Door && !x.Sacred && x.DryLand).Cast<Room>().ToList();
        var index = Random.Number(0, rooms.Count);

        index = (index == rooms.Count - 1) ? 0 : index + 1;

        return rooms[index];
    }

    private void RobYouBlind()
    {
        var wasLit = Lit;

        Lit = Query.Light();

        if (Lit || !wasLit)
        {
            return;
        }

        Print("The thief seems to have left you in the dark.");
    }

    private bool ThiefVsAdventurer(bool here)
    {
        var thief = this;
        var stiletto = Get<Stiletto>();

        bool robbed = false;
        bool robbedPlayer = false;
        bool foundAnything = false;

        if (!Flags.Dead && Location.Is<TreasureRoom>())
        {
            return false;
        }

        if (!Flags.ThiefHere)
        {
            if (!Flags.Dead && !here && 30 > Random.Number(100))
            {
                if (thief.Has(stiletto))
                {
                    thief.Concealed = false;

                    Flags.ThiefHere = true;

                    return Print("Someone carrying a large bag is casually leaning against " +
                        "one of the walls here. He does not speak, but it is clear from his " +
                        "aspect that the bag will be taken only over his dead body.");
                }

                if (!Player.Has(stiletto))
                {
                    return false;
                }

                stiletto.Move(thief);
                stiletto.Scenery = true;
                thief.Concealed = false;

                Flags.ThiefHere = true;

                return Print("You feel a light finger-touch, and turning, notice a grinning figure " +
                    "holding a large bag in one hand and a stiletto in the other.");
            }

            if (here && thief.Fight && !thief.TimeToRetreat())
            {
                thief.Concealed = true;
                thief.Fight = false;

                RetreiveStiletto();

                return Print("Your opponent, determining discretion to be the better part of valor, " +
                    "decides to terminate this little contretemps. With a rueful nod of his head, he " +
                    "steps backward into the gloom and disappears.");
            }

            if (here && thief.Fight && 90 > Random.Number(100))
            {
                return false;
            }

            if (here && 30 > Random.Number(100))
            {
                thief.Concealed = true;

                RetreiveStiletto();

                return Print("The holder of the large bag just left, looking disgusted. Fortunately, he took nothing.");
            }

            if (70 > Random.Number(100) || Flags.Dead)
            {
                return false;
            }

            if (Rob.Run(Location, thief, 100))
            {
                robbed = true;
            }
            else if (Rob.Run(player, thief))
            {
                robbed = true;
                robbedPlayer = true;
            }

            Flags.ThiefHere = true;

            if (robbed && !here)
            {
                Print("A seedy-looking individual with a large bag just wandered through the room. " +
                    "On the way through, he quietly abstracted some valuables from the room and from your " +
                    "possession, mumbling something about ~Doing unto others before...~");

                RobYouBlind();

                return false;
            }

            if (here)
            {
                RetreiveStiletto();

                if (robbed)
                {
                    var desc = "The thief just left, still carrying his large bag. You may not have noticed that he ";
                    if (robbedPlayer)
                    {
                        desc += "robbed you blind first.";
                    }
                    else
                    {
                        desc += "appropriated the valuables in the room.";
                    }

                    Print(desc);

                    RobYouBlind();
                }
                else
                {
                    Print("The thief, finding nothing of value, left disgusted.");
                }

                thief.Concealed = true;
                return true;
            }

            Print("A ~lean and hungry~ gentleman just wandered through, carrying a large bag. Finding nothing of value, he left disgruntled.");
        }

        if (!here && 30 < Random.Number(100))
        {
            return false;
        }

        foundAnything = Rob.Run(Location, thief, 100);

        robbed = (foundAnything ? foundAnything : Rob.Run(player, thief));

        if (robbed)
        {
            Print("The thief just left, still carrying his large bag. You may not have noticed that he robbed you blind first.");
            RobYouBlind();
        }
        else
        {
            Print("The thief, finding nothing of value, left disgusted.");
        }

        thief.Concealed = true;

        RetreiveStiletto();

        return false;
    }
}