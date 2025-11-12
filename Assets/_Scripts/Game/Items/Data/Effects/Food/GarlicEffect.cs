using System.Collections;
using UnityEngine;

public class GarlicEffect : MonoBehaviour, IItemEffect
{
    private float _effectCooldown = 4f;

    private float _armorBuffValue = 1f;

    private Character _targetCharacterToBuffArmor;

    private const float CHANCE_TO_REMOVE_VAMPIRISM_FROM_OPPONENT = 30f;
    private const int REMOVE_VAMPIRISM_AMOUNT = 2;

    private void Start()
    {
        CombatManager.Instance.OnCombatFinished += Combatmanager_OnCombatFinished;
    }

   
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatFinished -= Combatmanager_OnCombatFinished;
    }

    private void Combatmanager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        StopCoroutine(GarlicArmorRoutine());
    }

   
    public void ApplyEffect(ItemBehaviour item, Character targetCharacter)
    {      
        if (item == null)
            return;

        if (targetCharacter == null)
            return;

        _targetCharacterToBuffArmor = targetCharacter;

        _targetCharacterToBuffArmor.ChangeArmorValue(_armorBuffValue);

        bool isProcRemoveVampirism = UnityEngine.Random.Range(0,100) <= CHANCE_TO_REMOVE_VAMPIRISM_FROM_OPPONENT ? true : false;
        if (isProcRemoveVampirism)
            GetOpponentCharacter().RemoveBuff(Buff.BuffType.Vampirism, REMOVE_VAMPIRISM_AMOUNT);


        StartCoroutine(GarlicArmorRoutine());
       
    }

    public void RemoveEffect()
    {

    }
    private IEnumerator GarlicArmorRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_effectCooldown);
            _targetCharacterToBuffArmor.ChangeArmorValue(_armorBuffValue);

            bool isProcRemoveVampirism = UnityEngine.Random.Range(0, 100) <= CHANCE_TO_REMOVE_VAMPIRISM_FROM_OPPONENT ? true : false;
            if (isProcRemoveVampirism)
                GetOpponentCharacter().RemoveBuff(Buff.BuffType.Vampirism, REMOVE_VAMPIRISM_AMOUNT);
        }
    }

    private Character GetOpponentCharacter()
    {
        if(_targetCharacterToBuffArmor == PlayerCharacter.Instance)
        {
            return EnemyCharacter.Instance;
        }
        else if(_targetCharacterToBuffArmor == EnemyCharacter.Instance)
        {
            return PlayerCharacter.Instance;
        }
        else
        {
            Debug.Log("No target character to remove Vampirism find");
            return null;

        }
    }


}
