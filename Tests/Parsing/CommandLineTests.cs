using Zork1.Library.Parsing;

namespace Tests.Parsing;
public class CommandLineTests : BaseTestFixture
{
    [Fact]
    public void should_split_on_and_then()
    {
        var commands = CommandLine.GetCommands("open mailbox and then read leaflet");
        Assert.Equal(2, commands.Count);
        Assert.Equal("open mailbox", commands[0]);
        Assert.Equal("read leaflet", commands[1]);
    }

    [Fact]
    public void should_not_split_on_and()
    {
        var commands = CommandLine.GetCommands("take box and hat");
        Assert.Equal("take box hat", commands[0]);
    }

    [Fact]
    public void should_split_on_and()
    {
        var commands = CommandLine.GetCommands("open mailbox and take all");
        Assert.Equal("open mailbox", commands[0]);
        Assert.Equal("take all", commands[1]);
    }

    [Fact]
    public void should_split_on_period()
    {
        var commands = CommandLine.GetCommands("open mailbox.read leaflet.");
        Assert.Equal(2, commands.Count);
        Assert.Equal("open mailbox", commands[0]);
        Assert.Equal("read leaflet", commands[1]);
    }

    [Fact]
    public void actors_1()
    {
        // tell thief "drop knife"
        var commands = CommandLine.GetCommands("tell thief \"drop knife\"");
    }

    [Fact]
    public void actors_2()
    {
        // tell thief
        // drop knife
        var commands = CommandLine.GetCommands("thief, drop knife");
    }

    [Fact]
    public void actors_3()
    {
        // multi-command with talking - this syntax will run both commands
        // drop sword
        // tell troll
        // drop axe
        var commands = CommandLine.GetCommands("drop sword and tell troll \"drop axe\"");
    }

    [Fact]
    public void actors_4()
    {
        // multi-command with talking - this syntax discards everything after talk (drop sword not run)
        var commands = CommandLine.GetCommands("tell troll \"drop axe\" and drop sword");
    }

    [Fact]
    public void actors_5()
    {
        // empty tell
        // this will reach Before<Tell> on troll
        var commands = CommandLine.GetCommands("tell troll");
    }

    [Fact]
    public void can_say_things()
    {
        // tell me
        // hello
        var commands = CommandLine.GetCommands("say \"hello\"");
    }
}
