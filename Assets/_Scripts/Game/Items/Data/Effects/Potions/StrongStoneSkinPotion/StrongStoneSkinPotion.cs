using System;
using UnityEngine;

public class StrongStoneSkinPotion : MonoBehaviour, IItemEffect, IPotionEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;
    public event Action OnPotionConsumed;

    [SerializeField] private int _armorReachedToTriggerEffect = 45;

    [SerializeField] private float _covertedHealth = 15;
    [SerializeField] private int _armorBuff = 35;
    [SerializeField] private Buff _thornsBuff;

    private bool _canConsume = true;

    private ItemBehaviour _strongStoneSkinPotion;

    private void Awake()
    {
        _strongStoneSkinPotion = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        _strongStoneSkinPotion.OwnerCharacter.OnCharacterStatsChanged -= OwnerCharacter_OnCharacterStatsChanged;

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _strongStoneSkinPotion.OwnerCharacter.OnCharacterStatsChanged -= OwnerCharacter_OnCharacterStatsChanged;

        _canConsume = true;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_strongStoneSkinPotion == null || _strongStoneSkinPotion.OwnerCharacter == null) return;

        _strongStoneSkinPotion.OwnerCharacter.OnCharacterStatsChanged += OwnerCharacter_OnCharacterStatsChanged;

    }

    private void OwnerCharacter_OnCharacterStatsChanged(Character.CharacterStats stats)
    {
        if (stats.Armor >= _armorReachedToTriggerEffect && _canConsume)
        {
            TriggerEffect();
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void TriggerEffect()
    {
        _strongStoneSkinPotion.OwnerCharacter.TakeDamage(_covertedHealth, ItemDataSO.ExtraType.Effect);
        _strongStoneSkinPotion.OwnerCharacter.AddArmor(_armorBuff);
        _strongStoneSkinPotion.OwnerCharacter.ApplyBuff(_thornsBuff);

        OnActivate();
        OnPotionConsumed?.Invoke();
        _canConsume = false;
    }

    public void TriggerPotionEffect()
    {
        _strongStoneSkinPotion.OwnerCharacter.TakeDamage(_covertedHealth, ItemDataSO.ExtraType.Effect);
        _strongStoneSkinPotion.OwnerCharacter.AddArmor(_armorBuff);
        _strongStoneSkinPotion.OwnerCharacter.ApplyBuff(_thornsBuff);

        OnActivate();

    }
}
