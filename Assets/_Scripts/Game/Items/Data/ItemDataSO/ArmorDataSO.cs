using UnityEngine;

[CreateAssetMenu(fileName = "ArmorDataSO", menuName = "Items/Armor Data")]
public class ArmorDataSO :ItemDataSO
{
    public int SocketsAmount => _socketsAmount;
    [SerializeField] private int _socketsAmount;
}
