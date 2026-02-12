using System;
using UnityEngine;

public class StoneSkinPotionEffect : MonoBehaviour, IItemEffect, IPotionEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;
    public event Action OnPotionConsumed;

    [SerializeField] private int _armorReachedToTriggerEffect = 45;

    [SerializeField] private float _covertedHealth = 15;
    [SerializeField] private int _armorBuff = 30;

    private bool _canConsume = true;

    private ItemBehaviour _stoneSkinPotion;

    private void Awake()
    {
        _stoneSkinPotion = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        _stoneSkinPotion.OwnerCharacter.OnCharacterStatsChanged -= OwnerCharacter_OnCharacterStatsChanged;

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _stoneSkinPotion.OwnerCharacter.OnCharacterStatsChanged -= OwnerCharacter_OnCharacterStatsChanged;

        _canConsume = true;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_stoneSkinPotion == null || _stoneSkinPotion.OwnerCharacter== null) return;

        _stoneSkinPotion.OwnerCharacter.OnCharacterStatsChanged += OwnerCharacter_OnCharacterStatsChanged;

    }

    private void OwnerCharacter_OnCharacterStatsChanged(Character.CharacterStats stats)
    {
        if (stats.Armor >= _armorReachedToTriggerEffect)
        {
            Consume();
        }
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void Consume()
    {
        _stoneSkinPotion.OwnerCharacter.TakeDamage(_covertedHealth, ItemDataSO.ExtraType.Effect);
        _stoneSkinPotion.OwnerCharacter.AddArmor(_armorBuff);

        OnActivate();
        OnPotionConsumed?.Invoke();
        _canConsume = false;
    }

    public void TriggerPotionEffect()
    {
        _stoneSkinPotion.OwnerCharacter.TakeDamage(_covertedHealth, ItemDataSO.ExtraType.Effect);
        _stoneSkinPotion.OwnerCharacter.AddArmor(_armorBuff);

        OnActivate();
    }
}
