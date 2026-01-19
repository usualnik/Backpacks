using UnityEngine;

public class ArtifactStroneOfColdEffect : MonoBehaviour, IItemEffect
{
    [SerializeField] private Buff _stoneOfColdBuff;

    private Character _owner;
    private Character _target;
    private WeaponBehaviour _weaponBehaviour;

    private void Awake()
    {
        _weaponBehaviour = GetComponent<WeaponBehaviour>();
    }

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        CombatManager.Instance.AttackCharacterOnce(sourceCharacter, targetCharacter, _weaponBehaviour);
        targetCharacter.ApplyBuff(_stoneOfColdBuff);
    } 
    
}
