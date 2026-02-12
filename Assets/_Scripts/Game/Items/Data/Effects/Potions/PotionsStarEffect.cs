using UnityEngine;

public class PotionsStarEffect : MonoBehaviour, IStarEffect
{

    private IItemEffect _effect;
    private ItemBehaviour _starredPotion;

    private void Awake()
    {
        _effect = GetComponent<IItemEffect>();
    }
    private void Start()
    {
        _effect.OnEffectAcivate += Effect_OnEffectAcivate;
    }
    private void OnDestroy()
    {
        _effect.OnEffectAcivate -= Effect_OnEffectAcivate;

    }
    private void Effect_OnEffectAcivate()
    {
        if(CombatManager.Instance.IsInCombat)
        {
            _starredPotion.TryGetComponent(out IPotionEffect _effect);

            if (_effect != null)
            {
                _effect.TriggerPotionEffect();
            }
        }
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _starredPotion = targetItem;        
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _starredPotion = null;
    }
}
