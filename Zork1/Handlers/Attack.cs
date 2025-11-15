using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Melee;
using Zork1.Things;

namespace Zork1.Handlers;

public class Attack : Combat
{
    public override bool Handler(Object noun, Object second)
    {
        if (!noun.Animate)
        {
            return Print($"I've known strange people, but fighting {noun.IName}?");
        }

        if (second == null || second is PairOfHands)
        {
            return Print($"Trying to attack a {noun} with your bare hands is suicidal.");
        }

        if (!Player.Has(second))
        {
            return Print($"You aren't even holding {second.DName}.");
        }

        if (!second.Weapon)
        {
            return Print($"Trying to attack {noun.DName} with {second.IName} is suicidal.");
        }

        HeroBlow(noun, second);

        return true;
    }
}

//[ AttackSub o;
//  if( noun hasnt animate )
//    "I've known strange people, but fighting ", (a) noun, "?";
//  if( ~~second ) {
//    o = FindOneItemWithAttr(player, weapon);
//    if( o ) {
//      print "(with ", (the) o, ")^";
//      << Attack noun o >>;
//    }
//  } else
//    o = second;
//  if( ~~o || o == pair_of_hands )
//    "Trying to attack ", (a) noun, " with your bare hands is suicidal.";
//  if( o notin player )
//    "You aren't even holding ", (the) o, ".";
//  if( o hasnt weapon )
//    "Trying to attack ", (the) noun, " with ", (a) o, " is suicidal.";
//  if( o == rusty_knife ) 
//    return JigsUp("As the knife approaches its victim, your mind is submerged 
//                   by an overmastering will. Slowly, your hand turns, until 
//                   the rusty blade is an inch from your neck. The knife seems 
//                   to sing as it savagely slits your throat.");
//  if( RunLife(noun, ##Attack) )
//    rtrue;
//  HeroBlow();
//  rtrue;
//];

public abstract class Combat : Sub
{
    protected CombatOutcome HeroBlow(Object noun, Object second)
    {
        if (noun is Adventurer)
        {
            JigsUp("If you insist.... Poof, you're dead.");
            return CombatOutcome.Killed;
        }

        var villain = (Villain)noun;
        var playerWeapon = second;

        villain.Fight = true;

        // Inform: if (player has staggered) { ... give player ~staggered; rtrue; }
        if (player.Staggered)
        {
            player.Staggered = false;
            Print("You are still recovering from that last blow, so your attack is ineffective.");
            return CombatOutcome.Missed;
        }

        var playerStrength = Player.FightStrength(true);

        if (playerStrength < 1)
        {
            playerStrength = 1;
        }

        var villainStrength = villain.FightStrength(playerWeapon);
        var villainWeapon = villain.GetWeapon();

        CombatOutcome result;

        // Inform: if ((~~dweapon) || def < 0) { ... res = 3; }
        if (villainWeapon == null || villainStrength < 0) // Villain unarmed or unconscious/dead
        {
            Print($"The {(villainWeapon == null ? "unarmed " : "")}{(villainStrength < 0 ? "unconscious " : "")}{villain.Name} cannot defend himself: He dies.");
            result = CombatOutcome.Killed;
        }
        else
        {
            result = MeleeDefinitions.GetDefensiveResult(villainStrength, playerStrength);

            // Inform: if (nohesitate) { if (res == 6) { res = 8; } else { res = 9; } }
            // 'noHesitate' forces more decisive outcomes (Hesitate or SittingDuck)
            //if (noHesitate)
            //{
            //    result = result == CombatOutcome.Stagger ? CombatOutcome.Hesitate : CombatOutcome.SittingDuck;
            //}

            // Inform: if (res == 6 && dweapon && 25 > random(100)) { res = 7; }
            if (result == CombatOutcome.Stagger && villainWeapon != null && Random.Between(1, 100) <= 25)
            {
                result = CombatOutcome.LoseWeapon;
            }

            // Inform: Remark(PickOne(Hero_melee-->(res)),noun,second);
            // noun is villain, second is hero's weapon.
            var heroMessage = MeleeDefinitions.HeroMelee.AttackTypes[(int)result - 1].Messages.Pick();

            MeleeRoutine.Remark(heroMessage, second, villain);
        }

        // Inform: if (res ~= 1) { ... } (Apply effects if not a MISS)
        if (result != CombatOutcome.Missed)
        {
            if (result == CombatOutcome.Hesitate) { /* no code */ }
            else if (result == CombatOutcome.Knockout) { villain.Strength = -Math.Abs(villain.Strength); } // Make strength negative
            else if (result == CombatOutcome.Killed || result == CombatOutcome.SittingDuck) { villain.Strength = 0; } // Dead
            else if (result == CombatOutcome.LightWound) { villain.Strength = Math.Max(0, villain.Strength - 1); }
            else if (result == CombatOutcome.SeriousWound) { villain.Strength = Math.Max(0, villain.Strength - 2); }
            else if (result == CombatOutcome.Stagger) { villain.Staggered = true; }
            else if (result == CombatOutcome.LoseWeapon)
            {
                // Inform: give dweapon ~scenery; give dweapon weapon; move dweapon to location; SetPronoun('it', dweapon);
                if (villainWeapon != null)
                {
                    villainWeapon.Scenery = false;
                    villainWeapon.Weapon = true;
                    villainWeapon.Move(Player.Location);
                    SetLast.Object(villainWeapon);
                }
            }
        }
        return VillainResult(villain, villain.Strength, result);
    }

    public static CombatOutcome VillainResult(Villain villain, int currentStrength, CombatOutcome result)
    {
        // Inform: villain.strength = def;
        villain.Strength = currentStrength;

        // Inform: if (~~def) { ... remove villain; villain.fight(2); return res; }
        if (currentStrength <= 0) // If villain is dead
        {
            Print($"^Almost as soon as the {villain.Name} breathes his last breath, a cloud of sinister black fog envelops him, and when the fog lifts, the carcass has disappeared.");
            villain.Fight = false;
            villain.Remove();
            villain.FightAction(CombatFlags.DEAD); // Call villain's special death routine
            return result;
        }

        // Inform: if (res == 2) { villain.fight(3); return res; }
        if (result == CombatOutcome.Knockout)
        {
            villain.FightAction(CombatFlags.OUTCOLD); // Call villain's unconscious routine
        }

        return result;
    }
}