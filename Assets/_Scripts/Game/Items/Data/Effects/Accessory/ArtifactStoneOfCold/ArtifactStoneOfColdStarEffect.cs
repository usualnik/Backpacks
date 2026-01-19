using UnityEngine;

public class ArtifactStoneOfColdStarEffect : MonoBehaviour, IStarEffect
{

    [SerializeField] private Buff _stoneOfColdStarBuff;

    private WeaponBehaviour _weaponBehaviour;

    private void Start()
    {
        CombatManager.Instance.OnDamageDealt += CombatManager_OnDamageDealt;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnDamageDealt -= CombatManager_OnDamageDealt;

    }
    private void CombatManager_OnDamageDealt(WeaponBehaviour weapon, Character targetCharacter, float arg3)
    {
        if (_weaponBehaviour == null)
            return;

        if (_weaponBehaviour == weapon)
        {
            targetCharacter.ApplyBuff(_stoneOfColdStarBuff);
        }
    }

    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _weaponBehaviour = targetItem as WeaponBehaviour;
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        _weaponBehaviour = null;
    }
}
