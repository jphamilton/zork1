using Zork1.Library;
using Zork1.Library.Parsing;
using Zork1.Library.Things;
using Zork1.Rooms;
using Zork1.Scenic;
using Zork1.Things;

namespace Tests.Parsing;
public class LexerTests : BaseTestFixture
{
    [Fact]
    public void should_handle_partial_command()
    {
        // because at this stage we are not validating syntax!
        var x = Lexer.Tokenize("put on", null);
        Assert.False(x.IsError);
    }

    [Fact]
    public void cant_see_that()
    {
        var x = Lexer.Tokenize("take sack", null);
        Assert.Contains(Messages.CantSeeThatHere("sack"), x.Error);
    }

    [Fact]
    public void should_tokenize_simple_valid_input()
    {
        var x = Lexer.Tokenize("open mailbox", null);
        Assert.True(x.Error == null);
    }

    [Fact]
    public void take_all_should_not_reach_into_container()
    {
        var x = Lexer.Tokenize("open mailbox", null);
        Assert.True(x.Error == null);
    }

    [Fact]
    public void should_find_2_commands()
    {
        var leaflet = Here<Advertisement>();

        var commands = CommandLine.GetCommands("open mailbox and then read leaflet");
        var f1 = Lexer.Tokenize(commands[0], null);
        Assert.Equal("open", f1.Verb);
        Assert.Contains(Get<Mailbox>(), f1.Objects);
        Assert.False(f1.IsError);

        var f2 = Lexer.Tokenize(commands[1], null);
        Assert.Equal("read", f2.Verb);
        Assert.Contains(Get<Advertisement>(), f2.Objects);
        Assert.False(f2.IsError);
    }

    [Fact]
    public void should_not_split_into_2_commands()
    {
        var hat = Here<RedHat>();
        var box = Here<OpaqueBox>();

        var commands = CommandLine.GetCommands("take box and hat");
        var f1 = Lexer.Tokenize(commands[0], null);

        Assert.Contains(box, f1.Objects);
        Assert.Contains(hat, f1.Objects);
    }

    [Fact]
    public void should_reduce_2_adjectives_to_single_word()
    {
        var cl = CommandLine.GetCommands("open the small mailbox");
        var f = Lexer.Tokenize(cl[0], null);
        Assert.Equal("open", f.Verb);
        Assert.Contains(Get<Mailbox>(), f.Objects);
    }

    [Fact]
    public void should_reduce_lots_of_adjectives_to_single_word()
    {
        Get<Mailbox>().Open = true;
        var cl = CommandLine.GetCommands("read advertisement leaflet booklet");
        var f = Lexer.Tokenize(cl[0], null);
        Assert.Equal("read", f.Verb);
        Assert.Equal(1, f.Objects.Count(y => y is Advertisement));
    }

    [Fact]
    public void there_is_no_verb_in_this_sentence()
    {
        Get<Mailbox>().Open = true;
        var f = GetFrame("leaflet");
        Assert.Equal(Messages.NoVerbInSentence, f.Error);
    }

    [Fact]
    public void should_form_correct_sentence_from_trash()
    {
        Get<Mailbox>().Open = true;
        var f = GetFrame("leaflet read");
        Assert.Equal("read", f.Verb);
        Assert.Contains(Get<Advertisement>(), f.Objects);
    }

    [Fact]
    public void should_reduce_lots_of_adjectives_to_single_word_2()
    {
        var f = GetFrame("open small mailbox");
        Assert.Equal(1, f.Objects.Count(x => x is Mailbox));
    }

    [Fact]
    public void should_handle_except_with_not_found_object()
    {
        var f = GetFrame("open mailbox except leaflet");
        Assert.Equal("open", f.Verb);
        Assert.Contains(Get<Mailbox>(), f.Objects);
        Assert.DoesNotContain(Get<Advertisement>(), f.Objects);
    }


    [Fact]
    public void which_do_you_mean()
    {
        var red = Here<RedHat>();
        var black = Here<BlackHat>();
        var white = Here<WhiteHat>();
        var f = GetFrame("take hat");
        Assert.True(f.Orphan);
        Assert.Equal(3, f.UnresolvedObjects[0].Count);
        //Assert.StartsWith("Which do you mean", f.Error);
    }

    [Fact]
    public void should_swap_indirect_objects()
    {
        Get<Mailbox>().Open = true;
        var leaflet = Player.Add<Advertisement>();
        var f = GetFrame("put down leaflet");
        // parser sees "down" and then will assume "lamp" is indirect object, test for swap
        Assert.Contains(Get<Advertisement>(), f.Objects);
        Assert.Equal("down", f.Prep);
    }

    [Fact]
    public void used_verb_in_a_way_not_understood()
    {
        var f = GetFrame("open read mailbox");
        Assert.Equal("You used the word \"read\" in a way that I don't understand.", f.Error);
    }

    [Fact]
    public void used_verb_in_a_way_not_understood_2()
    {
        var f = GetFrame("open ignite mailbox");
        Assert.Equal("You used the word \"ignite\" in a way that I don't understand.", f.Error);
    }

    [Fact]
    public void prep_that_is_verb_should_work()
    {
        var f = GetFrame("down");
        Assert.Equal("down", f.Verb);
    }

    [Fact]
    public void inventory_should_be_available()
    {
        var box = Player.Add<OpaqueBox>();
        var f = GetFrame("drop box");
        Assert.Contains(box, f.Objects);
    }

    [Fact]
    public void take_all()
    {
        var f = GetFrame("take all");
        Assert.DoesNotContain(Get<Mailbox>(), f.Objects);
        Assert.True(f.All);
    }
    
    [Fact]
    public void indirect_partial_1()
    {
        var x = SetupPartialTest("put rock into hat", "black")[0];
        Assert.Contains(Get<MoonRock>(), x.Objects);
        Assert.Contains(Get<BlackHat>(), x.IndirectObjects);
    }

    [Fact]
    public void indirect_partial_2()
    {
        var x = SetupPartialTest("put rock into hat", "black hat and then open mailbox");
        Assert.Equal(2, x.Count);
        var a = x[0];
        var b = x[1];
        Assert.Contains(Get<MoonRock>(), a.Objects);
        Assert.Contains(Get<BlackHat>(), a.IndirectObjects);
    }

    [Fact]
    public void indirect_partial_3()
    {
        var x = SetupPartialTest("put rock into hat", "black and open mailbox");
        Assert.Equal(2, x.Count);
        var a = x[0];
        var b = x[1];
        Assert.Contains(Get<MoonRock>(), a.Objects);
        Assert.Contains(Get<BlackHat>(), a.IndirectObjects);
    }

    [Fact]
    public void indirect_partial_4()
    {
        var x = SetupPartialTest("put rock into hat", "put rock into black hat");
        var a = x[0];
        Assert.Contains(Get<MoonRock>(), a.Objects);
        Assert.Contains(Get<BlackHat>(), a.IndirectObjects);
    }

    [Fact]
    public void indirect_partial_5()
    {
        var x = SetupPartialTest("put rock into hat", "voltron").Last();
        Assert.Equal(Messages.DontKnowThatWord("voltron"), x.Error);
    }

    [Fact]
    public void object_partial_1()
    {
        var x = SetupPartialTest("take hat", "donkey").Last();
        Assert.Equal(Messages.DontKnowThatWord("donkey"), x.Error);
    }

    [Fact]
    public void object_partial_2()
    {
        var x = SetupPartialTest("take hat", "red hat and then open mailbox");
        Assert.Equal(2, x.Count);
        var a = x[0];
        var b = x[1];
        Assert.Equal("take", a.Verb);
        Assert.Contains(Get<RedHat>(), a.Objects);
    }

    [Fact]
    public void object_partial_3()
    {
        var x = SetupPartialTest("take hat", "red and open mailbox");
        Assert.Equal("take", x[0].Verb);
        Assert.Contains(Get<RedHat>(), x[0].Objects);
        Assert.Equal("open", x[1].Verb);
        Assert.Contains(Get<Mailbox>(), x[1].Objects);
    }

    [Fact]
    public void object_partial_4()
    {
        var x = SetupPartialTest("take hat","take red hat");
        Assert.Single(x);
        var a = x[0];
        Assert.Contains(Get<RedHat>(), a.Objects);
    }

    [Fact]
    public void object_partial_5()
    {
        var x = SetupPartialTest("take hat", "red and then open mailbox");
        Assert.Equal(2, x.Count);
        var a = x[0];
        var b = x[1];
        Assert.Contains(Get<RedHat>(), a.Objects);
        Assert.Contains(Get<RedHat>(), a.Objects);
    }

    [Fact]
    public void object_partial_6()
    {
        var x = SetupPartialTest("take hat", "red");
        Assert.Single(x);
        var a = x[0];
        Assert.Contains(Get<RedHat>(), a.Objects);
    }

    [Fact]
    public void object_partial_7()
    {
        var x = SetupPartialTest("take hat and rock", "red");
        Assert.Single(x);
        var a = x[0];
        Assert.Contains(Get<RedHat>(), a.Objects);
        Assert.Contains(Get<MoonRock>(), a.Objects);
    }

    [Fact]
    public void object_partial_8()
    {
        var red = Here<RedHat>();
        var black = Here<BlackHat>();
        var white = Here<WhiteHat>();

        var f = Lexer.Tokenize("take hat", null);
        Assert.True(f.Orphan);
        Assert.Equal(3, f.UnresolvedObjects[0].Count);

        f = Lexer.Tokenize("hat", f);
        Assert.True(f.Orphan);
        Assert.Equal(3, f.UnresolvedObjects[0].Count);

        f = Lexer.Tokenize("red", f);
        Assert.False(f.Orphan);
        Assert.Equal("take", f.Verb);
        Assert.Contains(Get<RedHat>(), f.Objects);
    }

    private List<Frame> SetupPartialTest(string partial, string response)
    {
        var results = new List<Frame>();
        var red = Here<RedHat>();
        var black = Here<BlackHat>();
        var white = Here<WhiteHat>();
        var rock = Here<MoonRock>();
        var cl = CommandLine.GetCommands(partial);
        var f = Lexer.Tokenize(cl[0], null);
        Assert.True(f.Orphan);

        cl = CommandLine.GetCommands(response);

        foreach (var c in cl)
        {
            f = Lexer.Tokenize(c, f);
            results.Add(f);
        }

        return results;
    }

    [Fact]
    public void should_handle_directions()
    {
        var f = GetFrame("n");
        Assert.Equal("north", f.Verb);
    }

    [Fact]
    public void should_go_direction()
    {
        var f = GetFrame("go n");
        Assert.Equal("north", f.Verb);
    }

    [Fact]
    public void should_handle_adverbs()
    {
        var torch = Here<FlickeringTorch>();
        var mailbox = Here<Mailbox>();
        var f = GetFrame("burn down mailbox with torch");
        Assert.Equal("burn down", f.Verb);
        Assert.Contains(mailbox, f.Objects);
        Assert.Contains(torch, f.IndirectObjects);
    }

    [Fact]
    public void should_handle_global_objects()
    {
        var f = GetFrame("count blessings");
        Assert.Equal("count", f.Verb);
        Assert.Contains(Get<Blessings>(), f.Objects);
    }

    [Fact]
    public void should_not_find_CARRIED()
    {
        var box = Inv<OpaqueBox>();
        box.Open = false;

        var cloak = Get<BlackCloak>();
        cloak.Move(box);

        var f = GetFrame("drop cloak");

        Assert.DoesNotContain(cloak, f.Objects);
    }

    [Fact]
    public void throw_swap()
    {
        Location = Get<InfiniteWhiteRoom>();
        var lunch = Inv<Lunch>();
        var f = GetFrame("throw lunch overboard");
        Assert.Contains(lunch, f.Objects);
        Assert.Contains(Get<SetOfTeeth>(), f.IndirectObjects);
    }

    [Fact]
    public void give_swap()
    {
        Location = Get<CyclopsRoom>();
        var lunch = Inv<Lunch>();
        var garlic = Inv<CloveOfGarlic>();
        var cyclops = Get<Cyclops>();

        var f = GetFrame("give cyclops lunch and garlic");

        Assert.Contains(lunch, f.Objects);
        Assert.Contains(garlic, f.Objects);
        Assert.Contains(cyclops, f.IndirectObjects);
    }

    [Fact]
    public void hand_swap()
    {
        Location = Get<CyclopsRoom>();
        var lunch = Inv<Lunch>();
        var garlic = Inv<CloveOfGarlic>();
        var cyclops = Get<Cyclops>();

        var f = GetFrame("hand cyclops lunch and garlic");

        Assert.Contains(lunch, f.Objects);
        Assert.Contains(garlic, f.Objects);
        Assert.Contains(cyclops, f.IndirectObjects);
    }

    [Fact]
    public void BUG_partial_frame_should_be_cleared_on_new_verb()
    {
        var matches = Inv<Matchbook>();
        var frame = Lexer.Tokenize("burn match"); // currently a partial
        
        SyntaxCheck.Check(frame, out Grammar grammar);
        Snarf.Objects(frame, grammar);

        frame = Lexer.Tokenize("light match", frame);
        Assert.Equal("light", frame.Verb);
        Assert.Contains(matches, frame.Objects);
        Assert.DoesNotContain(matches, frame.IndirectObjects);
    }

    [Fact]
    public void should_favor_object_over_verb()
    {
        var lamp = Here<BrassLantern>();
        var frame = Lexer.Tokenize("take light"); // light is also a verb
        Assert.Contains(lamp, frame.Objects);
    }

    [Fact]
    public void indirect_is_also_verb()
    {
        var plastic = Here<PileOfPlastic>();
        var pump = Inv<AirPump>();

        var frame = Lexer.Tokenize("inflate plastic with pump"); // pump is also a verb
        Assert.Contains(pump, frame.IndirectObjects);
    }

    [Fact]
    public void should_find_boat_when_player_in_boat()
    {
        Get<PileOfPlastic>().Remove();
        var river4 = Get<River4>();
        var boat = Get<MagicBoat>();
        boat.Move(river4);
        player.Parent = boat;

        var frame = Lexer.Tokenize("leave boat");
        Assert.Null(frame.Error);
        Assert.Contains(boat, frame.Objects);
    }

    private Frame GetFrame(string input)
    {
        var cl = CommandLine.GetCommands(input);
        return Lexer.Tokenize(cl[0], null);
    }

    [Fact]
    public void should_handle_number_object()
    {
        Inv<BlackBook>();
        var number = Get<Number>();
        var frame = Lexer.Tokenize("read page 100");
        Assert.Contains(number, frame.IndirectObjects);
        Assert.Equal(100, number.Value);
    }

    [Fact]
    public void should_handle_unresolve_objects_with_resolved_indirect()
    {
        // trophy case contains "gold" coffin
        // and player is inserting pot of "gold"
        Location = Get<LivingRoom>();
        var coffin = Get<GoldCoffin>();
        var trophy = Get<TrophyCase>();
        trophy.Open = true;
        coffin.Move(trophy);
        var pot = Inv<PotOfGold>();
        pot.Concealed = false;
        var frame = Lexer.Tokenize("put gold in case");
        var unresolvedObjects = frame.UnresolvedObjects.SelectMany(x => x.Objects).ToList();
        // pot of gold and coffin are in unresolved objects
        Assert.Contains(pot, unresolvedObjects);
        Assert.Contains(coffin, unresolvedObjects);
        // trophy case is resolved indirect object
        Assert.Contains(trophy, frame.IndirectObjects);
    }

    [Fact]
    public void BUG_kill_thief()
    {
        var frame = Lexer.Tokenize("kill");
        SyntaxCheck.Check(frame, out Grammar grammar);
        Assert.Equal("What do you want to kill?", frame.Error);
        var next = Lexer.Tokenize("thief", frame);
        Assert.Equal("You can't see any thief here!", next.Error);
    }

    [Fact]
    public void BUG_which_water()
    {
        // we are next to water and we have
        // a bottle full of water...two waters!
        Location = Get<Shore>();
        var bottle = Inv<GlassBottle>();
        bottle.Open = true;
        var frame = Lexer.Tokenize("fill bottle with water");
        Assert.Contains(bottle, frame.Objects);
        Assert.Empty(frame.UnresolvedObjects);
        Assert.Single(frame.UnresolvedIndirectObjects);
    }
}