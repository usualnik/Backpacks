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

    // ������� ������
    private Dictionary<ItemEffectSO, int> activeBuffs = new Dictionary<ItemEffectSO, int>();

    public void AddBuff(ItemEffectSO buff, int charges)
    {
        if (activeBuffs.ContainsKey(buff))
        {
            // ������� ������ ������ ����� ����������� ������
            buff.RemoveEffect(this, activeBuffs[buff]);
            activeBuffs[buff] = charges;
        }
        else
        {
            activeBuffs[buff] = charges;
        }

        // ��������� ������
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

    // ����� ��� ����� (��� ���������� � �����)
    public void OnMeleeAttack(Enemy enemy, int baseDamage)
    {
        int finalDamage = baseDamage + damageBonus;

        // ��������� ���������
        if (vampirismAmount > 0)
        {
            float healAmount = Mathf.Min(vampirismAmount, finalDamage * maxVampirismPercentage);
            health += healAmount;
        }

        // ��������� ���� (�������������� ����)
        if (thornsDamage > 0)
        {
            float thornsAmount = Mathf.Min(thornsDamage, finalDamage * maxThornsPercentage);
            enemy.TakeDamage((int)thornsAmount);
        }
    }
}
