using UnityEngine;

public abstract class StarEffectSO : ScriptableObject
{
    public ItemDataSO.ItemType Target => _target;
    public float Amount => _amount;

    [SerializeField] private string _name;
    [SerializeField][TextArea] private string _description;

    [SerializeField] ItemDataSO.ItemType _target;
    [SerializeField] private float _amount;

    public abstract void ApplyStarEffect(ItemBehaviour targetItem);
    public abstract void RemoveStarEffect(ItemBehaviour targetItem);
}
