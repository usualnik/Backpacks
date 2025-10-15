using UnityEngine;

[CreateAssetMenu(fileName = "SwordEffectSO", menuName = "Items/Effects/Sword")]
public class TestSwordEffectSO : ItemEffectSO
{
    public override void ApplyEffect(ItemBehaviour.Target target)
    {
      // No logic effect on sword
    }
}
