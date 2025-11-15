using Zork1.Library;

namespace Zork1.Handlers;

public class Open : Sub
{
    public override bool Handler(Object noun, Object second)
    {
        if (noun is Container container && container.Capacity > 0)
        {
            if (container.Open)
            {
                return Print("It is already open.");
            }

            container.Open = true;
            container.Visited = true;

            var contents = container.Items;

            if (contents.Count == 0 || container.Transparent)
            {
                return Print("Opened.");
            }

            if (contents.Count == 1 && !contents[0].Visited)
            {
                var obj = contents[0];
                var reveal = obj.Initial;

                if (reveal != null)
                {
                    SetLast.Object(obj);
                    Print($"The {container} opens.");
                    return Print($"^{reveal}");
                }
            }

            return container.PrintContents($"Opening the {container} reveals");
        }

        if (noun is Door door)
        {
            if (door.Open)
            {
                Print("It is already open.");
            }
            else
            {
                Print($"The {door} opens.");
                door.Open = true;
            }

            return true;
        }

        return Print($"You must tell me how to do that to a {noun}.");
    }
}
