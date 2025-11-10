using UnityEngine;

[CreateAssetMenu(fileName = "SpikedShieldEffectSO", menuName = "Items/Effects/Shields/SpikedShieldEffect")]

public class SpikedShieldEffectSO : ItemEffectSO
{
    public override void ApplyEffect(ItemBehaviour itemBehaviour)
    {
        CombatManager.Instance.ApplyEffect(itemBehaviour, this);
    }
}
