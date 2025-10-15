using UnityEngine;

[CreateAssetMenu(fileName = "ShieldEffectSO", menuName = "Items/Effects/Shield")]
public class TestShieldEffectSO : ItemEffectSO
{
    public override void ApplyEffect(ItemBehaviour.Target target)
    {
        CombatManager.Instance.ApplyEffect(target, this);

    }


}
