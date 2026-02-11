using System;
using UnityEngine;

public class LeatherArmorEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private int _armorBuff = 45;
    [SerializeField] private int _debuffResistsAmount = 3;


    private ItemBehaviour _leatherArmor;

    private void Awake()
    {
        _leatherArmor = GetComponent<ItemBehaviour>();
    }


    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if(_leatherArmor == null || _leatherArmor.OwnerCharacter == null) { return; }

        _leatherArmor.OwnerCharacter.AddArmor(_armorBuff);
        _leatherArmor.OwnerCharacter.AddDebuffResistStacks(_debuffResistsAmount);        
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

}


