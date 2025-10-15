using UnityEngine;

[CreateAssetMenu(fileName = "CloverEffectSO", menuName = "Items/Effects/Clover")]
public class TestCloverEffectSO : ItemEffectSO
{    
    public override void ApplyEffect(ItemBehaviour.Target target)
    {
        CombatManager.Instance.ApplyEffect(target, this);
    }

}
