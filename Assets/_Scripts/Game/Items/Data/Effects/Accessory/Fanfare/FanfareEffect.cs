using System;
using System.Collections;
using UnityEngine;

public class FanfareEffect : MonoBehaviour, IItemEffect, ICooldownable
{
    public int ItemActivations { get; set; }
    public float BaseCooldown { get; private set; } = 3f;
    public float CooldownMultiplier { get; set; } = 1f;
    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    public event Action OnEffectAcivate;

    [SerializeField] private Buff _empowerBuff;
    [SerializeField] private Buff _manaBuff;
    [SerializeField] private int _removeManaFromOpponentAmount = 2;
    [SerializeField] private float _removeStaminaAmount = 1f;
       
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

        CooldownMultiplier = 1f;
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


            OnActivate();

            yield return new WaitForCooldown(this);
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
       
}
