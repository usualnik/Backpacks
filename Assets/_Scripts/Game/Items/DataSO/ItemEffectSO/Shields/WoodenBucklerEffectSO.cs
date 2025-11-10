using UnityEngine;

[CreateAssetMenu(fileName = "WoodenBucklerEffectSO", menuName = "Items/Effects/Shields/WoodenBucklerEffect")]
public class WoodenBucklerEffectSO : ItemEffectSO
{
    public override void ApplyEffect(ItemBehaviour itemBehaviour)
    {
        CombatManager.Instance.ApplyEffect(itemBehaviour, this);

    }


}
