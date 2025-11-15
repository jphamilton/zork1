using Zork1.Melee;
using Zork1.Things;

namespace Zork1.Library.Things;

public abstract class Villain : Object
{
    public VillainDefinitionEntry Definition => MeleeDefinitions.VillainDefinitions.Single(x => x.Villain == this);

    public bool FightAction(CombatFlags flags)
    {
        switch (flags)
        {
            case CombatFlags.DISARMED:
                return OnDisarmed();
            case CombatFlags.DEAD:
                return OnKilled();
            case CombatFlags.OUTCOLD:
                return OnKnockedOut();
            case CombatFlags.WAKE:
                return OnWakeUp();
            case CombatFlags.ATTACKED:
                return OnAttacked();
        }

        return false;
    }

    //DISARMED = 1,   // M_HANDLED
    //DEAD = 2,       // M_ENTER
    //OUTCOLD = 3,    // M_LOOK
    //WAKE = 4,       // M_WAKE
    //ATTACKED = 5    // M_FIGHT
    public virtual bool OnDisarmed()
    {
        return false;
    }

    public virtual bool OnKilled()
    {
        return false;
    }

    public virtual bool OnKnockedOut()
    {
        return false;
    }

    public virtual bool OnWakeUp()
    {
        return false;
    }

    public virtual bool OnAttacked()
    {
        return false;
    }

    public Object GetWeapon()
    {
        return Items.FirstOrDefault(x => x.Weapon);
    }

    public bool CheckStrength()
    {
        if (Strength >= 0)
        {
            return true;
        }

        Strength = 0 - Strength;

        OnWakeUp();

        return true;
    }

    public bool TimeToRetreat()
    {
        int villainStrength = Strength;
        int playerEffectiveStrength = villainStrength - Player.FightStrength(true);

        if (playerEffectiveStrength > 3) { return Random.Between(1, 100) <= 90; }
        if (playerEffectiveStrength > 0) { return Random.Between(1, 100) <= 75; }
        if (playerEffectiveStrength == 0) { return Random.Between(1, 100) <= 50; }
        if (villainStrength > 1) { return Random.Between(1, 100) <= 25; }

        return Random.Between(1, 100) <= 10;
    }

    public int FightStrength(Object playerWeapon = null)
    {
        int effectiveStrength = Strength;

        if (this is Thief thief && thief.Engrossed)
        {
            if (effectiveStrength > 2)
            {
                effectiveStrength = 2;
            }

            thief.Engrossed = false;
        }

        if (playerWeapon == null)
        {
            playerWeapon = Context.Second?.Weapon == true ? Context.Second : null;
        }

        if (playerWeapon != null && Definition.EffectiveWeapon == playerWeapon)
        {
            effectiveStrength -= Definition.InitialStrength;

            if (effectiveStrength < 1)
            {
                effectiveStrength = 1;
            }
        }

        return effectiveStrength;
    }

    public bool TryTakeWeapon(Object weapon)
    {
        if (!Location.Has(this) || Concealed)
        {
            return false;
        }

        if (Has(weapon))
        {
            return Print($"The {Name} swings it out of your reach.");
        }

        return Print($"The {weapon} seems white-hot. You can't hold on to it.");
    }
}
