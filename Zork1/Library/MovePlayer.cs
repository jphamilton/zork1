using Zork1.Handlers;
using Zork1.Library.Locations;
using Zork1.Library.Things;
using Zork1.Things;

namespace Zork1.Library;
public static class MovePlayer
{
    public static bool To<T>(bool showRoomDesc = true) where T : Room
    {
        var room = Objects.Get<T>();
        To(room, showRoomDesc);
        return true;
    }

    public static bool To(Room room, bool showRoomDesc = true)
    {
        Player.Location = Move(room, showRoomDesc);
        return true;
    }

    private static Room Move(Room room, bool showRoomDesc = true)
    {
        // this is the actual room the player will be in, even though he could be moving between "Darkness" rooms
        Room realRoom = room;
        var goingToDryLand = room.DryLand;

        if (room.RMUNGBIT)
        {
            Output.Print(room.Description);
            return Player.Location;
        }

        if (Player.InVehicle)
        {
            var boat = Player.Parent;
            var onWater = boat.Parent.WaterRoom;
            
            if (goingToDryLand && !onWater)
            {
                // you're in a boat, but the boat is on shore
                Output.Print($"You can't go there in a {boat}.");
                return (Room)boat;
            }

            if (goingToDryLand && !Player.Location.DryLand && !Flags.Dead)
            {
                // landing on shore
                Output.Print($"The {boat} comes to a rest on the shore.");
            }

            boat.Move(room);
        }
        else
        {
            if (!goingToDryLand)
            {
                // trying to move from land to water without boat
                Output.Print("You can't go there without a vehicle.");
                return Player.Location;
            }

            Player.Location = room;
        }

        var wasLit = State.Lit;

        var isLit = Query.Light(room);

        State.Lit = isLit;

        if (wasLit && !isLit)
        {
            Output.Print("^You have moved into a dark place.");
            room = Objects.Get<Darkness>();
            Player.Location = room;
        }

        // grue!!!
        if (!wasLit && !State.Lit && 80 > Random.Number(100))
        {
            if (Flags.GrueRepellent)
            {
                Output.Print("There are sinister gurgling noises in the darkness all around you!");
            }
            else
            {
                var location = Player.InVehicle ? Player.Parent.Name : "room";
                Player.JigsUp($"Oh, no! A lurking grue slithered into the {location} and devoured you!");
                return Player.Location;
            }
        }

        if (!room.Visited)
        {
            if (showRoomDesc)
            {
                CurrentRoom.Look(false, isLit);
            }

            if (room.Initial?.Invoke() == false)
            {
                return Player.Location;
            }
        }
        else
        {
            if (!isLit && room.Visited)
            {
                // in the dark
            }
            else
            {
                if (showRoomDesc)
                {
                    CurrentRoom.Look(false, isLit);
                }
            }
        }

        if (room.TakeValue > 0 && !room.Visited)
        {
            Score.ScoreObject(room);
        }

        room.Visited = isLit && !room.MazeRoom;

        return Player.InVehicle ? (Room)Player.Parent : realRoom;
    }
}
