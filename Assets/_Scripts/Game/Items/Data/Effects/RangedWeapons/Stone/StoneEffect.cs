using System;
using UnityEngine;

public class StoneEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private int _destroyArmorAmount = 4;

    private WeaponBehaviour _stone;    

    private void Awake()
    {
        _stone = GetComponent<WeaponBehaviour>();
    }
    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnHit;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnHit;
    }

    private void CombatManager_OnHit(WeaponBehaviour arg1, Character arg2, float arg3)
    {
        if (_stone == arg1)
        {
            OnHitEffect();
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        if (_stone.TargetCharacter == null) return;

        CombatManager.Instance.AttackCharacterOnce(_stone.OwnerCharacter, _stone.TargetCharacter, _stone);
    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
  

    private void OnHitEffect()
    {
        _stone.TargetCharacter.AddArmor(-_destroyArmorAmount);
        OnActivate();
    }
}
