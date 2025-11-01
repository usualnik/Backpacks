using UnityEngine;

[CreateAssetMenu(fileName = "ShieldDataSO", menuName = "Items/Shield Data")]
public class ShieldDataSO : ItemDataSO
{
    public int SocketsAmount => _socketsAmount;
    [SerializeField] private int _socketsAmount;
    public override void PerformAction(ItemBehaviour.Target target)
    {     

    }
}
