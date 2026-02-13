using System;
using System.Collections;
using UnityEngine;

public class GarlicEffect : MonoBehaviour, IItemEffect, IFoodEffect, ICooldownable
{
    public event Action OnEffectAcivate;
    public int ItemActivations { get; set; }

    public float BaseCooldown { get; private set; } = 4f;
    public float CooldownMultiplier { get; set; } = 1f;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }
  
    [SerializeField]
    private float _armorBuffValue = 1f;
    [SerializeField]
    private float _chanceToRemoveVampirism = 30f;
    [SerializeField]
    private int _removeVampirismAmount = 2;

    private Character _targetCharacterToBuffArmor;

    private Coroutine _garlicRoutine;


    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += Combatmanager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= Combatmanager_OnCombatFinished;

        if (_garlicRoutine != null)
            StopCoroutine(_garlicRoutine);
    }

    private void Combatmanager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_garlicRoutine != null)
        {
            StopCoroutine(_garlicRoutine);
            _garlicRoutine = null;
        }

        CooldownMultiplier = 1f;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (item == null || targetCharacter == null)
            return;

        _targetCharacterToBuffArmor = targetCharacter;

        _targetCharacterToBuffArmor.AddArmor(_armorBuffValue);
        TryRemoveVampirism();

        if (_garlicRoutine != null)
            StopCoroutine(_garlicRoutine);

        _garlicRoutine = StartCoroutine(GarlicArmorRoutine());
    }

    public void RemoveEffect()
    {
        if (_garlicRoutine != null)
        {
            StopCoroutine(_garlicRoutine);
            _garlicRoutine = null;
        }
    }

    private IEnumerator GarlicArmorRoutine()
    {
        while (true)
        {
            yield return new WaitForCooldown(this);

            _targetCharacterToBuffArmor.AddArmor(_armorBuffValue);
            TryRemoveVampirism();
            OnActivate();

        }
    }

    private void TryRemoveVampirism()
    {
        bool isProcRemoveVampirism = UnityEngine.Random.Range(0, 100) <= _chanceToRemoveVampirism;
        if (isProcRemoveVampirism)
        {
            Character opponent = GetOpponentCharacter();
            if (opponent != null)
                opponent.RemoveBuff(Buff.BuffType.Vampirism, _removeVampirismAmount);
        }
    }

    private Character GetOpponentCharacter()
    {
        if (_targetCharacterToBuffArmor == PlayerCharacter.Instance)
            return EnemyCharacter.Instance;
        else if (_targetCharacterToBuffArmor == EnemyCharacter.Instance)
            return PlayerCharacter.Instance;
        else
        {
            Debug.LogWarning("No target character to remove Vampirism found");
            return null;
        }
    }
    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void TriggerFoodEffect()
    {
        TryRemoveVampirism();
        OnActivate();
    }
 
}