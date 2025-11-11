using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffectData", order = 0)]
public class ItemEffectSO : ScriptableObject
{    
    [SerializeField] private string _name;
    [SerializeField] [TextArea] private string _description;
    [SerializeField] private EffectType _effectType;
    [SerializeField] private bool _isPositive;
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
        Armor,

        //Debuffs
        Poison,
        Blindness,
        Cold,
    }
    public string Name => _name;
    public string Description => _description;
    public EffectType Type => _effectType;
    public bool IsPositive => _isPositive;     

}
