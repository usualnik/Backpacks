using UnityEngine;

[CreateAssetMenu(fileName = "SpikedShieldEffectSO", menuName = "Items/Effects/Shields/SpikedShieldEffect")]

public class SpikedShieldEffectSO : ItemEffectSO
{
    public override void ApplyEffect(ItemBehaviour.Target target)
    {
        CombatManager.Instance.ApplyEffect(target, this);
    }
}
