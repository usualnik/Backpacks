using UnityEngine;

[CreateAssetMenu(fileName = "WalrusTuskEffectSO", menuName = "Items/Effects/Accessory/WalrusTuskEffect")]
public class WalrusTuskEffectSO : ItemEffectSO
{
    public override void ApplyEffect(ItemBehaviour itemBehaviour)
    {
        CombatManager.Instance.ApplyEffect(itemBehaviour, this);
    }
}
