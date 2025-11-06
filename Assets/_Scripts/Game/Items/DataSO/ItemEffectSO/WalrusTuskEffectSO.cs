using UnityEngine;

[CreateAssetMenu(fileName = "WalrusTuskEffectSO", menuName = "Items/Effects/Accessory/WalrusTuskEffect")]
public class WalrusTuskEffectSO : ItemEffectSO
{
    public override void ApplyEffect(ItemBehaviour.Target target)
    {
        CombatManager.Instance.ApplyEffect(target, this);
    }
}
