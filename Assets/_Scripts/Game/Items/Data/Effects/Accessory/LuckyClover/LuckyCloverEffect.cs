using UnityEngine;

public class LuckyCloverEffect : MonoBehaviour, IItemEffect
{

    public void ApplyEffect(ItemBehaviour target, ItemEffectSO effectData)
    {
        CombatManager.Instance.ApplyEffect(target, effectData);
    }

    public void RemoveEffect()
    {
    }
}
