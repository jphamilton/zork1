using Zork1.Handlers;
using Zork1.Library;
using Zork1.Library.Functions;
using Zork1.Library.Things;
using Zork1.Rooms;
using Zork1.Scenic;

namespace Zork1.Things;

public class MagicBoat : Room
{
    private Dictionary<Room, Room> NearWater = [];

    public bool Deflated { get; set; } = true;

    public MagicBoat()
    {
        Capacity = 100;
        Size = 20;
        Search = true;
        Open = true;
        Takeable = true;
        Flammable = true;
        Vehicle = true;
    }

    public override void Initialize()
    {
        Name = "magic boat";
        Adjectives = ["boat", "raft", "inflatable", "magic", "plastic", "seaworthy"];
        IsHere<TanLabel>();

        var (dam_base, beach2, beach1, shore) = Get<DamBase, Beach2, Beach1, Shore>();
        var (sandy_beach, reservoir_south, reservoir_north, stream_view) = Get<SandyBeach, ReservoirSouth, ReservoirNorth, StreamView>();
        var (river4, river2, frigid_river, river1) = Get<River4, River2, FrigidRiver, River1>();
        var (reservoir, in_stream) = Get<Reservoir, InStream>();

        NearWater = new Dictionary<Room, Room>
        {
            { dam_base, river4 },
            { beach2, river2 },
            { beach1, frigid_river },
            { shore, river1 },
            { sandy_beach, frigid_river },
            { reservoir_south, reservoir },
            { reservoir_north, reservoir },
            { stream_view, in_stream }
        };

        Before<Fill, Inflate>(() => Print("Inflating it further would probably burst it."));
        Before<Deflate>(() =>
        {
            if (Player.Parent == this)
            {
                return Print("You can't deflate the boat while you're in it.");
            }

            if (!Location.Has(this))
            {
                return Print("The boat must be on the ground to be deflated.");
            }

            Deflated = true;

            Remove();

            var plastic = MoveHere<PileOfPlastic>();
            SetLast.Object(plastic);

            return Print("The boat deflates.");
        });

        Before<Launch>(() =>
        {
            if (!Has(player))
            {
                return Print("You're not in the boat!");
            }

            if (NearWater.TryGetValue(Location, out var room))
            {
                var river = Get<River>();

                GoTo(room);

                river.Start(room);
            }

            return true;
        });

        Before<Go>(() =>
        {
            return false;
            //if( noun == land_obj or e_obj or w_obj or d_obj )
            //        rtrue;
            //    if( location == reservoir && (noun == n_obj or s_obj) )
            //        rtrue;
            //    if( location == stream && noun == s_obj )
            //        rtrue;
            //    print "Read the label for the boat's instructions.^";
        });

        Before<Drop>(() =>
        {
            if (Noun.Weapon)
            {
                return PunctureBoat(Noun);
            }

            return false;
        });

        Before<Insert>(() =>
        {
            if (Noun.Weapon && Second == this)
            {
                return PunctureBoat(Noun);
            }

            return false;
        });

        Before<Poke, Attack>(() =>
        {
            if (Second.Weapon)
            {
                return PunctureBoat(Second);
            }

            return false;
        });

        Before<Board>(() =>
        {
            var weapons = Player.Children.Any(x => x.Weapon);

            if (weapons)
            {
                var punctured_boat = Get<PuncturedBoat>();
                Remove();
                punctured_boat.Move(Location);
                SetLast.Object(punctured_boat);
                return Print("Oops! Something sharp seems to have slipped and punctured the boat. The boat deflates to the sounds of hissing, sputtering, and cursing.");
            }

            return false;
        });
    }

    private bool PunctureBoat(Object obj)
    {
        Remove();
        MoveHere<PileOfPlastic>();
        Rob.Run(this, Location);
        player.Move(Location);

        Print($"It seems that the {obj} didn't agree with the boat, as evidenced by the loud hissing " +
            "noise issuing therefrom. With a pathetic sputter, the boat finishes deflating, leaving you without.");

        if (Location.WaterRoom)
        {
            return JigsUp("In other words, fighting the fierce currents of the Frigid River. You manage to hold your " +
                "own for a bit, but then you are carried over a large waterfall and into some nasty rocks. Ouch!");
        }

        return true;
    }
}

public class TanLabel : Object
{
    public TanLabel()
    {
        Readable = true;
        Takeable = true;
        Flammable = true;
        Size = 2;
    }

    public override void Initialize()
    {
        Name = "tan label";
        Adjectives = ["label", "tan", "fineprint", "print", "fine"];
        Text = "    !!!!  FROBOZZ MAGIC BOAT COMPANY  !!!!^^" +
            "Hello, Sailor!^^" +
            "Instructions for use:^^" +
            "  To get into a body of water, say ~Launch~.^" +
            "  To get to shore, say ~Land~ or the direction in which you want to maneuver the boat.^^" +
            "Warranty:^^" +
            "  This boat is guaranteed against all defects for a period of 76 milliseconds from date of purchase or until first used, whichever comes first.^^" +
            "Warning:^" +
            "  This boat is made of thin plastic.^" +
            "Good Luck!";
    }
}

public class PileOfPlastic : Object
{
    public PileOfPlastic()
    {
        Takeable = true;
        Flammable = true;
        Size = 20;
    }

    public override void Initialize()
    {
        Name = "pile of plastic";
        Adjectives = ["boat", "pile", "plastic", "valve", "inflatable"];
        Description = "There is a folded pile of plastic here which has a small valve attached.";
        Before<Fill, Inflate>(() =>
        {
            if (!Location.Has(this))
            {
                return Print("The boat must be on the ground to be inflated.");
            }

            if (Second is AirPump)
            {
                Print("The boat inflates and appears seaworthy.");

                var label = Get<TanLabel>();

                if (!label.Visited)
                {
                    Print("A tan label is lying inside the boat.");
                }

                Remove();

                var boat = MoveHere<MagicBoat>();
                boat.Deflated = false;

                return SetLast.Object(boat);
            }

            if (Second is BlastOfAir)
            {
                return Print("You don't have enough lung power to inflate it.");
            }

            return Print($"With a {Second}? Surely you jest!");
        });
    }
}

public class PuncturedBoat : Object
{
    public PuncturedBoat()
    {
        Size = 20;
        Takeable = true;
        Flammable = true;
    }

    public override void Initialize()
    {
        Name = "punctured boat";
        Adjectives = ["boat", "pile", "plastic", "punctured", "large"];
        Before<PutOn, Insert>(() =>
        {
            if (Noun is ViscousMaterial)
            {
                return RepairBoat();
            }

            return false;
        });

        Before<Fill, Inflate>(() => Print("This boat will not inflate since some moron punctured it."));

        Before<Fix>(() =>
        {
            if (Second is ViscousMaterial)
            {
                return RepairBoat();
            }

            return Print($"With a {Second}?");
        });
    }

    private bool RepairBoat()
    {
        Remove();
        MoveHere<PileOfPlastic>();
        return Print("Well done. The boat is repaired.");
    }
}