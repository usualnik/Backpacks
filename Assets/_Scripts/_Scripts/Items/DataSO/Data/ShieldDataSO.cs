using UnityEngine;

[CreateAssetMenu(fileName = "ShieldDataSO", menuName = "Items/Shield Data")]
public class ShieldDataSO : ItemDataSO
{
    public int SocketsAmount => _socketsAmount;
    public ItemType Type => ITEM_TYPE;


    [SerializeField] private int _socketsAmount;

    private const ItemType ITEM_TYPE = ItemType.Shields;

    public override void PerformAction(ItemBehaviour.Target target)
    {     
        Effect.ApplyEffect();
    }
}
