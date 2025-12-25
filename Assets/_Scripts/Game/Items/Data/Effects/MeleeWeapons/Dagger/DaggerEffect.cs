using UnityEngine;

public class DaggerEffect : MonoBehaviour, IItemEffect
{
    private WeaponBehaviour _daggerBehaviour;

    private void Awake()
    {
        _daggerBehaviour = GetComponent<WeaponBehaviour>();
    }
    private void Start()
    {
       CombatManager.Instance.OnCharacterStuned += CombatManager_OnCharacterStuned;
    }
    private void OnDestroy()
    {
      CombatManager.Instance.OnCharacterStuned -= CombatManager_OnCharacterStuned;
    }
    private void CombatManager_OnCharacterStuned(Character stunnedCharacter, float stunDuration)
    {
        if (!_daggerBehaviour.OwnerCharacter)
        {
            return;
        }

        if (stunnedCharacter != null && stunnedCharacter == _daggerBehaviour.OwnerCharacter)
        {
            CombatManager.Instance.AttackCharacterOnce(_daggerBehaviour.OwnerCharacter, _daggerBehaviour.TargetCharacter, _daggerBehaviour);
        }
    }

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    public void RemoveEffect()
    {
    }
}
