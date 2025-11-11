using UnityEngine;

public class BloodAmuletEffect : MonoBehaviour, IItemEffect
{
    private float _additionalHealth = 20f;
    public void ApplyEffect(ItemBehaviour target, ItemEffectSO effectData)
    {
       Character targetCharacter = target.GetTarget() == ItemBehaviour.Target.Player 
            ? PlayerCharacter.Instance : EnemyCharacter.Instance;

        CombatManager.Instance.ApplyEffect(target, effectData);
        targetCharacter.BuffHealth(_additionalHealth);
    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }
}
