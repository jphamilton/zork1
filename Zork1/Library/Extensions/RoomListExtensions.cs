using Zork1.Library.Things;

namespace Zork1.Library.Extensions;

public static class RoomListExtensions
{
    public static Room GetNext(this List<Room> rooms, bool wrap = true)
    {
        var index = rooms.Count > 0 ? rooms.IndexOf(Player.Location) : -1;

        if (index < 0)
        {
            return null;
        }

        index++;

        if (index >= rooms.Count - 1)
        {
            if (wrap)
            {
                index = 0;
            }
            else
            {
                return null;
            }
        }

        return rooms[index];
    }

    public static bool GoNext(this List<Room> rooms, bool wrap = true)
    {
        var room = GetNext(rooms, wrap);

        if (room == null)
        {
            return false;
        }

        MovePlayer.To(room);
        return true;
    }

}
