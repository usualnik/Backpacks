using UnityEngine;

[CreateAssetMenu(fileName = "BagDataSO", menuName = "Items/Bag Data")]
public class BagDataSO : ItemDataSO
{
    public int SlotsAmount => _slotsAmount;

    [SerializeField] private int _slotsAmount;


    public override void PerformAction(ItemBehaviour.Target target)
    {
       
    }
}
