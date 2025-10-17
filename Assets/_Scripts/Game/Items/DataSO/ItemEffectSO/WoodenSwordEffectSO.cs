using UnityEngine;

[CreateAssetMenu(fileName = "WoodenSwordEffectSO", menuName = "Items/Effects/MeleeWeapons/WoodenSwordEffect")]
public class WoodenSwordEffectSO : ItemEffectSO
{
    public override void ApplyEffect(ItemBehaviour.Target target)
    {
      // No effect on sword
    }
}
