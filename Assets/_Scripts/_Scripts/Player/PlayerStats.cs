using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float health = 100;
    public float accuracy = 1f;
    public int damageBonus = 0;
    public float itemCooldownReduction = 0f;
    public float itemCooldownIncrease = 0f;
    public int thornsDamage = 0;
    public int vampirismAmount = 0;
    public float maxMana = 0;
    public float currentMana = 0;

    [Header("Limits")]
    public float maxThornsPercentage = 1f;
    public float maxVampirismPercentage = 1f;

    // Система баффов
    private Dictionary<ItemEffectSO, int> activeBuffs = new Dictionary<ItemEffectSO, int>();

    public void AddBuff(ItemEffectSO buff, int charges)
    {
        if (activeBuffs.ContainsKey(buff))
        {
            // Убираем старый эффект перед применением нового
            buff.RemoveEffect(this, activeBuffs[buff]);
            activeBuffs[buff] = charges;
        }
        else
        {
            activeBuffs[buff] = charges;
        }

        // Применяем эффект
        buff.ApplyEffect(this, charges);

        Debug.Log($"Applied {buff.BuffName} x{charges}");
    }

    public void RemoveBuff(ItemEffectSO buff)
    {
        if (activeBuffs.ContainsKey(buff))
        {
            buff.RemoveEffect(this, activeBuffs[buff]);
            activeBuffs.Remove(buff);
        }
    }

    public bool HasBuff(ItemEffectSO buff)
    {
        return activeBuffs.ContainsKey(buff);
    }

    public int GetBuffCharges(ItemEffectSO buff)
    {
        return activeBuffs.ContainsKey(buff) ? activeBuffs[buff] : 0;
    }

    // Метод для атаки (для вампиризма и шипов)
    public void OnMeleeAttack(Enemy enemy, int baseDamage)
    {
        int finalDamage = baseDamage + damageBonus;

        // Применяем вампиризм
        if (vampirismAmount > 0)
        {
            float healAmount = Mathf.Min(vampirismAmount, finalDamage * maxVampirismPercentage);
            health += healAmount;
        }

        // Применяем шипы (дополнительный урон)
        if (thornsDamage > 0)
        {
            float thornsAmount = Mathf.Min(thornsDamage, finalDamage * maxThornsPercentage);
            enemy.TakeDamage((int)thornsAmount);
        }
    }
}
