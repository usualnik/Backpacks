using UnityEngine;

public class DjinLampStarEffect : MonoBehaviour, IStarEffect
{  
    [SerializeField] private float _weaponDamageBuff = 27f;

    [Header("ConditionsToProc")]
    [SerializeField] private int _armorToProcNeeded = 7;
    [SerializeField] private int _luckToProcNeeded = 7;
    [SerializeField] private int _thornsToProcNeeded = 7;
    [SerializeField] private int _manaToprocNeeded = 7;
    [SerializeField] private float _hpToprocNeeded = 27f;


    private WeaponBehaviour _trackedWeapon;
    private DjinLampEffect _djinLampEffect;
    private ItemBehaviour _djinLamp;
    private bool _canProc = true;

    private void Awake()
    {
        _djinLampEffect = GetComponent<DjinLampEffect>();   
        _djinLamp = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        _djinLampEffect.OnEffectAcivate += DjinLampEffect_OnEffectAcivate;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;

    } 

    private void OnDestroy()
    {
        _djinLampEffect.OnEffectAcivate -= DjinLampEffect_OnEffectAcivate;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _canProc = true;
    }

    private void DjinLampEffect_OnEffectAcivate()
    {
        CheckForProcWeaponDamageBuff();
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _trackedWeapon = targetItem as WeaponBehaviour;
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _trackedWeapon = null;
    }

    private void CheckForProcWeaponDamageBuff()
    {
        if(!_canProc) return;
        if(_djinLamp.OwnerCharacter == null) return;
        if (_trackedWeapon == null) return;
        if (_djinLamp.OwnerCharacter.GetArmorValue() < _armorToProcNeeded) return;
        if (_djinLamp.OwnerCharacter.GetBuffStacks(Buff.BuffType.Luck) < _luckToProcNeeded) return;
        if (_djinLamp.OwnerCharacter.GetBuffStacks(Buff.BuffType.Thorns) < _thornsToProcNeeded) return;
        if (_djinLamp.OwnerCharacter.GetBuffStacks(Buff.BuffType.Mana) < _manaToprocNeeded) return;
        if (_djinLamp.OwnerCharacter.Stats.Health < _hpToprocNeeded) return;

        _djinLamp.OwnerCharacter.AddArmor(-_armorToProcNeeded);
        _djinLamp.OwnerCharacter.RemoveBuff(Buff.BuffType.Luck, _luckToProcNeeded);
        _djinLamp.OwnerCharacter.RemoveBuff(Buff.BuffType.Thorns, _thornsToProcNeeded);
        _djinLamp.OwnerCharacter.RemoveBuff(Buff.BuffType.Mana, _manaToprocNeeded);
        _djinLamp.OwnerCharacter.TakeDamage(_hpToprocNeeded, ItemDataSO.ExtraType.Effect);

        _trackedWeapon.AddDamageToWeapon(_weaponDamageBuff);

        _canProc = false;

    }



}
