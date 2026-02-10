using System;
using System.Collections;
using UnityEngine;

public class FanfareEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private Buff _empowerBuff;
    [SerializeField] private Buff _manaBuff;
    [SerializeField] private int _removeManaFromOpponentAmount = 2;
    [SerializeField] private float _removeStaminaAmount = 1f;
    [SerializeField] private float _cooldown = 3f;

    private float _currentCooldownMultiplier = 1f;

    private Coroutine _fanfareRoutine;
    private ItemBehaviour _fanfare;
    private void Awake()
    {
        _fanfare = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_fanfareRoutine != null)
        {
            StopCoroutine(FanfareRoutine());
            _fanfareRoutine = null;
        }

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_fanfareRoutine != null)
        {
            StopCoroutine(FanfareRoutine());
            _fanfareRoutine = null;
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_fanfareRoutine == null)
        {
            _fanfareRoutine = StartCoroutine(FanfareRoutine());
        }
    }

    private IEnumerator FanfareRoutine()
    {
        while (true)
        {
            ProcBuff();

            float currentCooldown = _cooldown / _currentCooldownMultiplier;

            OnActivate();

            yield return new WaitForSeconds(currentCooldown);
        }
    }

    private void ProcBuff()
    {
        if(_fanfare.OwnerCharacter == null) { return; }
        if(_fanfare.TargetCharacter == null) { return; }


        int randomEffectIndex = UnityEngine.Random.Range(0, 3);

        /*
         * 
         * Randomly gain 1 Empower                          // 0 
         * 
         * or gain 3 Mana and remove 2 Mana from opponent   // 1
         * 
         * or remove 1 stamina from opponent.               // 2
         * 
         */


        switch (randomEffectIndex)
        {
            case 0:
                _fanfare.OwnerCharacter.ApplyBuff(_empowerBuff);
                break;

            case 1:
                _fanfare.OwnerCharacter.ApplyBuff(_manaBuff);
                _fanfare.TargetCharacter.RemoveBuff(Buff.BuffType.Mana, _removeManaFromOpponentAmount);
                break;

            case 2:
                _fanfare.TargetCharacter.UseStamina(_removeStaminaAmount);
                break;
        }

    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void IncreaseSpeed(float increaseAmount)
    {
        _currentCooldownMultiplier += increaseAmount;

        if (_fanfareRoutine != null)
        {
            StopCoroutine(_fanfareRoutine);
            _fanfareRoutine = StartCoroutine(FanfareRoutine());
        }
    }
}
