using System;
using UnityEngine;

public class HammerEffect : MonoBehaviour, IItemEffect
{
    public int ItemActivations { get; set; }

    public event Action OnEffectAcivate;

    [SerializeField] private float _chanceToStun = 45f;
    [SerializeField] private float _stunDuration = 0.5f;


    private WeaponBehaviour _hammer;

    private void Awake()
    {
        _hammer = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnHit;
    }

    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnHit;
    }

    private void CombatManager_OnHit(WeaponBehaviour weapon, Character arg2, float arg3)
    {
        if (_hammer == weapon)
        {
            bool isStunProc = UnityEngine.Random.Range(1, 100) < _chanceToStun;

            if (isStunProc)
            {
                StunOpponent();
            }
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

    private void StunOpponent()
    {
        CombatManager.Instance.StunCharacter(_hammer.TargetCharacter, _stunDuration);
    }
}
