using System;
using UnityEngine;

public class ShovelEffect : MonoBehaviour, IItemEffect
{
    public event Action OnEffectAcivate;
    public int ItemActivations { get; set; }


    [SerializeField] private Buff _blindBuff;
    [SerializeField] private float _chanceToInflictBlind = 40f;

    private WeaponBehaviour _shovel;

    private void Awake()
    {
        _shovel = GetComponent<WeaponBehaviour>();
    }

    private void Start()
    {
        CombatManager.Instance.OnHit += CombatManager_OnHit;
        Shop.Instance.OnShopEnteredAfterCombat += Shop_OnShopEnteredAfterCombat;
    }

 
    private void OnDestroy()
    {
        CombatManager.Instance.OnHit -= CombatManager_OnHit;
        Shop.Instance.OnShopEnteredAfterCombat -= Shop_OnShopEnteredAfterCombat;
    }

    private void Shop_OnShopEnteredAfterCombat()
    {
        DigUpItem();
    }

    private void CombatManager_OnHit(WeaponBehaviour weapon, Character target, float arg3)
    {
        if (_shovel == weapon)
        {
            if (_shovel.TargetCharacter)
            {
                TryApplyBuff(_shovel.TargetCharacter);
            }
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

    private void DigUpItem()
    {
        ItemDataSO randomAvailavleItemData = Shop.Instance.GetRandomAvailableItemDataSO();

        GameObject diggedItem = randomAvailavleItemData.Prefab;

        Transform storageTransform = Storage.Instance.transform;

        if (diggedItem != null)
        {
            Instantiate(diggedItem, storageTransform, storageTransform);
        }
    }
   
}
