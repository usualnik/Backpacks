using UnityEngine;

public abstract class StarEffectSO : ScriptableObject
{
    public ItemDataSO.ItemType Target => _target;

    [SerializeField] private string _name;
    [SerializeField][TextArea] private string _description;

    [SerializeField] ItemDataSO.ItemType _target;

    public abstract void ApplyStarEffect();
}
