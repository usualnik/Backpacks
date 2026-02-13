using System;
using System.Collections;
using UnityEngine;

public class FluteEffect : MonoBehaviour, IItemEffect,ICooldownable
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    public float BaseCooldown { get; private set; } = 4.7f;
    public float CooldownMultiplier { get; set; } = 1f;

    public float CurrentCooldown
    {
        get
        {
            float safeMultiplier = Math.Max(0.01f, CooldownMultiplier);
            return MathF.Round(BaseCooldown / safeMultiplier, 3);
        }
    }

    [SerializeField] private Buff _luckBuff;
    [SerializeField] private float _staminaBuff = 2f;
    [SerializeField] private int _armorBuffAmount = 14;   

    private Coroutine _fluteRoutine;

    private ItemBehaviour _flute;

    private void Awake()
    {
        _flute = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

        if (_fluteRoutine != null)
        {
            StopCoroutine(FanfareRoutine());
            _fluteRoutine = null;
        }

    }
    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_fluteRoutine != null)
        {
            StopCoroutine(FanfareRoutine());
            _fluteRoutine = null;
        }

        CooldownMultiplier = 1f;
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_fluteRoutine == null)
        {
            _fluteRoutine = StartCoroutine(FanfareRoutine());
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
        if (_flute.OwnerCharacter == null) { return; }


        int randomEffectIndex = UnityEngine.Random.Range(0, 3);

        /*
         * 
         * Randomly gain 14 armor                         // 0 
         * 
         * or 2 stamina                                   // 1
         * 
         * or 2 luck                                      // 2
         * 
         */


        switch (randomEffectIndex)
        {
            case 0:
                _flute.OwnerCharacter.AddArmor(_armorBuffAmount);
                break;

            case 1:
                _flute.OwnerCharacter.AddStamina(_staminaBuff);
                break;

            case 2:
                _flute.OwnerCharacter.ApplyBuff(_luckBuff);
                break;
        }

    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
     
}
