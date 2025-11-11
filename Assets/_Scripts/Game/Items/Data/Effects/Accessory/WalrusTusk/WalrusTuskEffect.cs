using UnityEngine;

public class WalrusTuskEffect : MonoBehaviour, IItemEffect
{

    public void ApplyEffect(ItemBehaviour target, ItemEffectSO effectData)
    {
       CombatManager.Instance.ApplyEffect(target, effectData);
    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }
}
