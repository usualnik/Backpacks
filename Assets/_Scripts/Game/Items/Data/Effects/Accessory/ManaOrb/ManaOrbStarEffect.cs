using System.Collections.Generic;
using UnityEngine;

public class ManaOrbStarEffect : MonoBehaviour, IStarEffect
{

    [SerializeField] private List<ItemBehaviour> _trackedItems = new List<ItemBehaviour>();
    [SerializeField] private Buff _manaBuff;
    [SerializeField] private int _chaneToProcManaBuff = 50;
    [SerializeField] private int _newRandomBuffsAmount = 20;
    [SerializeField] private int _useManaAmountToProcRandomBuffs = 35;


    private ItemBehaviour _manaOrb;


    private void Awake()
    {
        _trackedItems = new List<ItemBehaviour>();
        _manaOrb = GetComponent<ItemBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }


    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;


    }
    private void CombatManager_OnCombatStarted()
    {
        TrackStaredItems();
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        StopTrackItems();
    }


    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_trackedItems.Contains(targetItem))
        {
            _trackedItems.Add(targetItem);
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_trackedItems.Contains(targetItem))
        {
            _trackedItems.Remove(targetItem);
        }
    }

    private void TrackStaredItems()
    {
        foreach (var item in _trackedItems)
        {
            item.GetComponent<IItemEffect>().OnEffectAcivate += TryGainManaBuff;
        }
    }

    private void StopTrackItems()
    {
        foreach (var item in _trackedItems)
        {
            item.GetComponent<IItemEffect>().OnEffectAcivate -= TryGainManaBuff;
        }
    }

    private void TryGainManaBuff()
    {
        bool isProc = UnityEngine.Random.Range(0, 101) < _chaneToProcManaBuff;

        if (!isProc) return;
        if(!_manaOrb.OwnerCharacter) return;

        _manaOrb.OwnerCharacter.ApplyBuff(_manaBuff);

        if (_manaOrb.OwnerCharacter.GetBuffStacks(Buff.BuffType.Mana) >= _useManaAmountToProcRandomBuffs)
        {
            GainRandomBuffs();
        }
    }

    private void GainRandomBuffs()
    {
        _manaOrb.OwnerCharacter.RemoveBuff(Buff.BuffType.Mana, _useManaAmountToProcRandomBuffs);

        Buff[] _randomBuffs = new Buff[20];

        for (int i = 0; i < _randomBuffs.Length; i++)
        {
            _randomBuffs[i] = GenerateRandomBuff();
        }

        for (int i = 0; i < _randomBuffs.Length; i++)
        {
            _manaOrb.OwnerCharacter.ApplyBuff(_randomBuffs[i]);
        }

        _randomBuffs = null;
    }
    private Buff GenerateRandomBuff()
    {
        int randomBuffTypeValue = UnityEngine.Random.Range(1, 8);

        Buff.BuffType randomBuffType = (Buff.BuffType) randomBuffTypeValue;

        return new Buff
        {
            Name = "ManaOrbRandomBuff",
            Type = randomBuffType,
            IsPositive = true,
            Value = 1
        };
    }

}
