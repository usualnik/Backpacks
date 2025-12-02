using UnityEngine;

[CreateAssetMenu(fileName = "HelmetDataSO", menuName = "Items/Helmet Data")]
public class HelmetDataSO : ItemDataSO
{
    public int SocketsAmount => _socketsAmount;
    [SerializeField] private int _socketsAmount;
}
