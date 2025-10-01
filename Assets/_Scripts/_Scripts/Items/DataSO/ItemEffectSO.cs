using UnityEngine;

public abstract class ItemEffectSO : ScriptableObject
{
    [SerializeField] protected string buffName;
    [SerializeField] protected Sprite icon;

    [Header("BUFF OR DEBUFF")]
    [SerializeField] protected bool isPositive; 

    public string BuffName => buffName;
    public Sprite Icon => icon;
    public bool IsPositive => isPositive;

    // Основные методы, которые должны реализовать все эффекты
    public abstract void ApplyEffect(PlayerStats player, int charges);
    public abstract void RemoveEffect(PlayerStats player, int charges);
    public abstract string GetDescription(int charges);

}
