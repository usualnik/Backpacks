using System;
using UnityEngine;

public class StaminaSackEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private float _staminaBuffAmount = 1f;


    private ItemBehaviour _staminaSack;

    private void Awake()
    {
        _staminaSack = GetComponent<ItemBehaviour>();
    }


    void Start()
    {
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        _staminaSack.OwnerCharacter.ChangeStaminaMaxValue(-_staminaBuffAmount);
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_staminaSack == null || _staminaSack.OwnerCharacter == null) return;

        _staminaSack.OwnerCharacter.ChangeStaminaMaxValue(_staminaBuffAmount);

        OnActivate();
    }

    

  
}
