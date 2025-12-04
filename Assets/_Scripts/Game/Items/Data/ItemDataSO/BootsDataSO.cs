using UnityEngine;

[CreateAssetMenu(fileName = "BootsDataSO", menuName = "Items/Boots Data")]
public class BootsDataSO : ItemDataSO
{
    public int SocketsAmount => _socketsAmount;
    [SerializeField] private int _socketsAmount;
}
