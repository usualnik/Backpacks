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
        Debug.Log(_daggerBehaviour.TargetCharacter);

    }
    private void OnDestroy()
    {
      CombatManager.Instance.OnCharacterStuned -= CombatManager_OnCharacterStuned;
    }
    private void CombatManager_OnCharacterStuned(Character arg1, float arg2)
    {
        Debug.Log(arg1);
    }

    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {

    }

    public void RemoveEffect()
    {
    }
}
