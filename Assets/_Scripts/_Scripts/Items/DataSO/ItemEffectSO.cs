using UnityEngine;

public abstract class ItemEffectSO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _description;

    public enum EffectType
    {
        //Buffs
        Luck,
        Regeneration,
        Thorns,
        Vampirism,
        Mana,
        Heat,
        Amplification,

        //Debuffs
        Poison,
        Blindness,
        Cold,
    }
}
