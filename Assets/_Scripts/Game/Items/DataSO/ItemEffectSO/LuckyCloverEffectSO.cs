using UnityEngine;

[CreateAssetMenu(fileName = "LuckyEffectSO", menuName = "Items/Effects/Accessory/LuckyCloverEffect")]
public class LuckyCloverEffectSO : ItemEffectSO
{    
    public override void ApplyEffect(ItemBehaviour.Target target)
    {
        CombatManager.Instance.ApplyEffect(target, this);
    }

}
