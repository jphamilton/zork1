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
        // thief
        // drop knife
        var commands = CommandLine.GetCommands("thief, drop knife");
    }
}
