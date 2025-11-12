[System.Serializable]
public struct Buff
{    
    public string Name;
    public BuffType Type;
    public bool IsPositive;
    public int Value;
    
    public enum BuffType
    {
        None,

        //Buffs
        Empower,
        Heat,
        Luck,
        Mana,
        Regeneration,
        Thorns,
        Vampirism,

        //Debuffs
        Poison,
        Blindness,
        Cold,
    }
}
