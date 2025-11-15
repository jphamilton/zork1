using Zork1.Handlers;
using Zork1.Library.Parsing;
using Zork1.Library.Things;
using Handler = System.Func<Zork1.Library.Object, Zork1.Library.Object, bool>;

namespace Zork1.Library;

public class Command
{
    private readonly Type _type;
    private readonly List<Object> _objects;
    private readonly Object _indirect;
    private readonly Handler _handler;
    private readonly Handler _pre;
    private readonly bool _all;

    private bool IsMany => _objects.Count > 1 || _all;

    public Command(Frame frame)
    {
        Context.Verb = frame.Sub;

        _type = frame.Sub.GetType();
        _handler = frame.Sub.Handler;
        _pre = frame.Sub.PreSub;
        _objects = [.. frame.Objects];
        _indirect = frame.IndirectObjects.FirstOrDefault(); //[.. frame.IndirectObject ];
        _all = frame.All;
    }

    private Command(List<Object> objects, Object indirect, Handler handler, Type type)
    {
        _objects = objects;
        _indirect = indirect;
        _handler = handler;
        _type = type;
    }

    private Command(Handler handler, Type type)
    {
        _handler = handler;
        _type = type;
    }

    // used by Redirect as part of a currently running command
    public static bool Run<T>(Object obj, Object indirect, Handler handler) where T : Sub
    {
        var type = typeof(T);
        var command = new Command([obj], indirect, handler, type);
        return command.HandleObjects();
    }

    // used by Redirect as part of a currently running command
    public static bool Run<T>(Handler handler) where T : Sub
    {
        var type = typeof(T);
        var command = new Command(handler, type);

        if (Context.Verb is Direction)
        {
            return command.HandleMove();
        }

        return command.HandleEmpty();
    }

    public CommandResult Run()
    {
        var result = new CommandResult
        {
            Success = false,
        };

        var success = false;

        try
        {
            if (Context.Verb is Direction)
            {
                success = HandleMove();
            }
            else if (_objects.Count > 0)
            {
                success = HandleObjects();
            }
            else
            {
                success = HandleEmpty();
            }

            result.Success = success;
        }
        catch (DeathException dx)
        {
            // player died somehow :(
            var dead = Routines.GetJigsUp();
            dead.DeathMessage = dx.Message;
            result.Success = dead.Handler(null, null);
        }

        Context.Verb = null;
        Context.Noun = null;
        Context.Second = null;

        return result;
    }

    private bool HandleEmpty()
    {
        var handled = HandleWinner();

        if (!handled)
        {
            handled = HandleBeforeRoom();

            if (!handled)
            {
                if (HandlePreAction(null, null))
                {
                    return true;
                }

                handled = HandleDefault(null);
            }
        }

        HandleAfterRoom();

        return handled;
    }

    private bool HandleObjects()
    {
        bool handled = false;

        Context.Second = _indirect;

        foreach (var obj in _objects)
        {
            Context.Noun = obj;

            if (IsMany)
            {
                Output.SetObject(obj);
            }

            handled = HandlePreAction(obj, _indirect);

            if (handled)
            {
                break;
            }

            handled = HandleWinner();

            if (handled)
            {
                break;
            }

            handled = HandleBeforeRoom();

            if (handled)
            {
                break;
            }

            handled = HandleBeforeIndirect();

            if (handled)
            {
                break;
            }

            handled = HandleBeforeObject(obj);

            if (!handled)
            {
                handled = HandleDefault(obj);

                if (handled)
                {
                    HandleAfterIndirect();
                    HandleAfterObject(obj);
                }
                else
                {
                    break;
                }
            }
        }

        Output.SetObject(null);

        HandleAfterRoom();

        return handled;
    }

    private bool HandlePreAction(Object obj, Object second)
    {
        if (_pre != null && _pre(obj, _indirect))
        {
            return true;
        }

        return false;
    }

    private bool HandleBeforeIndirect()
    {
        bool handled = false;

        if (_indirect != null)
        {
            var before = _indirect.GetBeforeRoutine(_type);

            if (before != null)
            {
                handled = before();
            }
        }

        return handled;
    }

    private bool HandleBeforeObject(Object obj)
    {
        bool handled = false;

        var before = obj.GetBeforeRoutine(_type);

        if (before != null)
        {
            handled = before();
        }

        if (!handled)
        {
            handled = HandleBeforeIndirect();
        }

        return handled;
    }

    private bool HandleDefault(Object obj)
    {
        return _handler(obj, _indirect);
    }

    private void HandleAfterObject(Object obj)
    {
        var after = obj.GetAfterRoutine(_type);

        if (after != null)
        {
            after();
        }

        HandleAfterIndirect();
    }

    private void HandleAfterIndirect()
    {
        if (_indirect != null)
        {
            var after = _indirect.GetAfterRoutine(_type);

            if (after != null)
            {
                after();
            }
        }
    }

    private bool HandleBeforeRoom(Room location = null)
    {
        location ??= Player.Location;

        var before = location.GetBeforeRoutine(_type);

        if (before != null)
        {
            return before();
        }

        return false;
    }

    private void HandleAfterRoom(Room location = null)
    {
        location ??= Player.Location;

        var after = Player.Location.GetAfterRoutine(_type);

        if (after != null)
        {
            after();
        }
    }

    private bool HandleWinner()
    {
        //var action = Player.Instance.GetBeforeRoutine(_type);
        var action = Player.Instance.GetBeforeRoutine(_type);

        if (action != null)
        {
            return action();
        }

        return false;
    }

    private bool HandleMove()
    {
        //var actor = Context.Winner;
        var actor = Player.Instance;
        var direction = (Direction)Context.Verb;
        var current = Player.Location;

        Room next = null;

        if (IsEnter())
        {
            if (IsDoor(out Door door))
            {
                next = door.GetOtherSide();

                if (next == null)
                {
                    // movement through door is blocked
                    return true;
                }
            }
            else
            {
                // something like "enter boat"
                return HandleObjects();
            }
        }

        next ??= current.GetRoomTo(direction);

        if (next == Player.Location)
        {
            // we haven't gone anywhere...for reasons
            return true;
        }

        if (next == null)
        {
            return true;
        }

        // Actor/Winner - before enter, before go
        var before = actor.GetBeforeRoutine(typeof(Enter)) ?? actor.GetBeforeRoutine(typeof(Go));
        if (before != null && before())
        {
            return true;
        }

        // Room - before enter, before go
        before = next.GetBeforeRoutine(typeof(Enter)) ?? next.GetBeforeRoutine(typeof(Go));
        if (before != null && before())
        {
            return true;
        }

        var handled = MovePlayer.To(next);

        if (handled)
        {
            // check previous room for Exit routine, we dont do things like After<North>
            var after = current.GetAfterRoutine(typeof(Exit));
            after?.Invoke();
        }

        return true;
    }

    private bool IsEnter()
    {
        return Context.Verb is Enter && _objects.Count > 0;
    }

    private bool IsDoor(out Door door)
    {
        door = null;

        if (IsEnter() && _objects[0] is Door d)
        {
            door = d;
            return true;
        }

        return false;
    }
}
