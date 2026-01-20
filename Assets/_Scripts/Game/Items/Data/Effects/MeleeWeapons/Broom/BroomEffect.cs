using System;
using UnityEngine;

public class BroomEffect : MonoBehaviour, IItemEffect
{
    public event Action OnEffectAcivate;
    public int ItemActivations { get; set; }


    [SerializeField] private Buff _blindBuff;
    [SerializeField] private float _chanceToInflictBlind = 33f;


    private WeaponBehaviour _weaponBehaviour;



    private void Awake()
    {
        _weaponBehaviour = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnDamageDealt += CombatManager_OnDamageDealt;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnDamageDealt -= CombatManager_OnDamageDealt;

    }
    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character target, float arg3)
    {
        if (_weaponBehaviour == weapon)
        {
            TryApplyBuff(target);
        }
    }

    private void TryApplyBuff(Character target)
    {
        bool isProc = UnityEngine.Random.Range(1, 100) <= _chanceToInflictBlind ? true : false;

        if (isProc)
        {
            target.ApplyBuff(_blindBuff);
            OnActivate();
        }
    }

    public void StartOfCombatInit(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    public void OnActivate()
    {
        ItemActivations++;
        OnEffectAcivate?.Invoke();
    }
}
