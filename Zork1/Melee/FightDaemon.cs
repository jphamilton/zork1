using Zork1.Library;
using Zork1.Library.Things;
using Zork1.Things;

namespace Zork1.Melee;

public class FightDaemon : Object
{
    public override void Initialize()
    {
        Daemon = TheFightDaemon;
    }

    private bool TheFightDaemon()
    {
        if (Flags.Dead)
        {
            return false;
        }

        bool fightInProgress = false;

        foreach (var villain in GetVillains())
        {
            var def = villain.Definition;

            if (villain is Thief thief && thief.Engrossed)
            {
                thief.Engrossed = false;
                continue;
            }

            if (villain.Strength < 0) // villain is unconscious
            {
                int wakeUpChance = def.WakeUpChance;

                if (wakeUpChance > 0 && Random.Between(1, 100) <= wakeUpChance)
                {
                    def.WakeUpChance = 0;       // reset chance
                    villain.CheckStrength();    // attempt to wake up
                    continue; // Skip attack if just woke up or still unconscious
                }

                def.WakeUpChance += 25; // Increase chance for next turn
                continue; // still unconscious, skip attack
            }

            if (!villain.Fight && !villain.FightAction(CombatFlags.ATTACKED))
            {
                continue; // if not fighting and can't be forced to, skip
            }

            fightInProgress = true; // at least one villain is fighting
        }

        if (!fightInProgress)
        {
            return false; // no active fighting
        }

        return DoFight(); // Proceed with actual combat rounds
    }

    private bool DoFight()
    {
        int consecutiveOutTurns = 0; // 'out' in Inform, counts turns player is knocked out

        do
        {
            bool anyVillainAttacked = false;

            foreach (var villain in GetVillains())
            {
                if (villain.FightAction(CombatFlags.DISARMED)) // if villain decides to disengage, skip
                {
                    continue;
                }

                CombatOutcome blowResult = VillainBlow(villain);

                anyVillainAttacked = true;

                if (blowResult == CombatOutcome.Knockout)
                {
                    // player knocked out for 1-3 turns
                    consecutiveOutTurns = 1 + Random.Between(1, 3);
                }
            }

            if (Flags.Dead)
            {
                return false;
            }

            // If no villains attacked this round and player isn't knocked out, stop fighting
            if (!anyVillainAttacked && consecutiveOutTurns == 0)
            {
                return true;
            }

            if (consecutiveOutTurns > 0)
            {
                consecutiveOutTurns--;
            }

        } while (consecutiveOutTurns > 0); // keep fighting if player is still knocked out

        return true; // fight finished (player is no longer out cold, or all active villains dealt with)
    }

    private static List<Villain> GetVillains()
    {
        return [.. Player.Location.Children.OfType<Villain>().Where(x => !x.Concealed)];
    }

    private CombatOutcome VillainBlow(Villain villain, bool outCold = false) // 'out' in Inform maps to 'outCold' here
    {
        var def = villain.Definition;
        var meleeData = def.MeleeData;

        player.Staggered = false;

        if (villain.Staggered)
        {
            Print($"The {villain} slowly regains his feet.");
            villain.Staggered = false;
            return CombatOutcome.Missed; // villain misses their turn while recovering
        }

        int villainStrength = villain.FightStrength();

        int playerEffectiveStrength = Player.FightStrength(true); // adjust for player's current strength

        if (playerEffectiveStrength <= 0)
        {
            // Player is already severely weakened or dead, villain's blow might automatically succeed or be less meaningful.
            // In Inform, this often means the villain proceeds to a fatal blow.
            // For simplicity, if player is already down, villain wins.
            return CombatOutcome.Killed; // player is defeated, villain gets automatic win
        }

        int playerBaseStrength = Player.FightStrength(false); // Player's strength before dynamic adjustments

        var playerWeapon = Context.Second?.Weapon == true ? Context.Second : null;

        CombatOutcome result;

        if (playerEffectiveStrength < 0)
        {
            result = CombatOutcome.Killed;
        }
        else
        {
            result = MeleeDefinitions.GetDefensiveResult(playerEffectiveStrength, villainStrength);

            if (outCold)
            {
                result = result == CombatOutcome.Stagger ? CombatOutcome.Hesitate : CombatOutcome.SittingDuck;
            }

            // if player is staggered and has a weapon, 25% chance to lose it instead.
            if (result == CombatOutcome.Stagger && playerWeapon != null && Random.Between(1, 100) <= 25)
            {
                result = CombatOutcome.LoseWeapon;
            }

            MeleeMessage villainMessage = meleeData.AttackTypes[(int)result - 1].Messages.Pick();

            MeleeRoutine.Remark(villainMessage, playerWeapon, player);
        }

        if (result != CombatOutcome.Missed)
        {
            if (result == CombatOutcome.Hesitate)
            {
                // no direct code effect here in original Inform, just a narrative outcome.
            }
            else if (result == CombatOutcome.Knockout)
            {
                // Inform: ! no code (Inform's strength handling for knockout is done in WinnerResult)
                // We'll signal this for WinnerResult.
            }
            else if (result == CombatOutcome.Killed || result == CombatOutcome.SittingDuck)
            {
                playerEffectiveStrength = 0; // Player is dead
            }
            else if (result == CombatOutcome.LightWound)
            {
                playerEffectiveStrength--;

                if (playerEffectiveStrength < 0)
                {
                    playerEffectiveStrength = 0;
                }

                State.LoadMax = Math.Max(50, State.LoadMax - 10);
            }
            else if (result == CombatOutcome.SeriousWound)
            {
                playerEffectiveStrength -= 2;

                if (playerEffectiveStrength < 0)
                {
                    playerEffectiveStrength = 0;
                }

                // Inform: if (Load_max > 50) { Load_max = Load_max - 20; }
                State.LoadMax = Math.Max(50, State.LoadMax - 20);
            }
            else if (result == CombatOutcome.Stagger)
            {
                player.Staggered = true;
            }
            else if (result == CombatOutcome.LoseWeapon)
            {
                if (playerWeapon != null)
                {
                    playerWeapon.Move(Player.Location); // drop weapon to current room

                    Object alternateWeapon = GetWeapon(player); // Check if player has other weapons (e.g., from inventory, not modeled here)

                    if (alternateWeapon != null)
                    {
                        Output.Print($"Fortunately, you still have a {alternateWeapon.Name}.");
                    }
                }
            }
        }

        return WinnerResult(playerEffectiveStrength, result, playerBaseStrength);
    }

    private static Object GetWeapon(Object actor) => actor.Get(x => x.Weapon);

    private CombatOutcome WinnerResult(int playerCurrentStrengthRaw, CombatOutcome combatOutcome, int playerOriginalStrength)
    {
        if (playerCurrentStrengthRaw <= 0)
        {
            player.Strength = -10000; // Deep dead state
        }
        else
        {
            player.Strength = playerCurrentStrengthRaw - playerOriginalStrength;
        }

        if (playerCurrentStrengthRaw < playerOriginalStrength) // If player took damage
        {
            Clock.Queue<HealFunction>(30);
        }

        if (Player.FightStrength(true) <= 0)
        {
            player.Strength = 1 - Player.FightStrength(false);
            JigsUp("It appears that that last blow was too much for you. I'm afraid you are dead.");
            return CombatOutcome.Killed;
        }

        return combatOutcome;
    }
}
