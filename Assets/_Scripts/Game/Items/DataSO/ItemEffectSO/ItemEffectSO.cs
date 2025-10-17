using UnityEngine;


public abstract class ItemEffectSO : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] [TextArea] private string _description;
    [SerializeField] private float _amount;
    [SerializeField] private EffectType _effectType;
    [SerializeField] private bool _isBuff;

    public enum EffectType
    {
        None,

        //Buffs
        Luck,
        Regeneration,
        Thorns,
        Vampirism,
        Mana,
        Heat,
        Amplification,
        ShieldEffect,
        BagEffect,

        //Debuffs
        Poison,
        Blindness,
        Cold,
    }
    public string Name => _name;
    public string Description => _description;
    public float Amount => _amount;
    public EffectType Type => _effectType;
    public bool IsBuff => _isBuff;
    public abstract void ApplyEffect(ItemBehaviour.Target target);
}
