using Zork1.Library.Things;
using Zork1.Library.Utilities;

namespace Zork1.Melee;

public class MeleeMessagePart
{
    public string Text { get; }
    public int? SpecialCode { get; }

    public MeleeMessagePart(string text) { Text = text; }
    public MeleeMessagePart(int specialCode) { SpecialCode = specialCode; }
}

public class MeleeMessage
{
    public List<MeleeMessagePart> Parts { get; } = [];
}

public class MeleeAttackType
{
    public PickOne<MeleeMessage> Messages { get; set; }
}

public class MeleeDefinition
{
    public List<MeleeAttackType> AttackTypes { get; } = [];
}

public class VillainDefinitionEntry
{
    public Villain Villain { get; }
    public Object EffectiveWeapon { get; }
    public int InitialStrength { get; }
    public int WakeUpChance { get; set; }
    public MeleeDefinition MeleeData { get; }

    public VillainDefinitionEntry(Villain villain, Object weapon, int initialStr, int wakeUpChance, MeleeDefinition meleeDef)
    {
        Villain = villain;
        EffectiveWeapon = weapon;
        InitialStrength = initialStr;
        WakeUpChance = wakeUpChance;
        MeleeData = meleeDef;
    }
}