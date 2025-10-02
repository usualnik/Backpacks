using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Items/Melee Weapon Data")]
public class MeleeWeaponDataSO : ItemDataSO
{
    public ItemType Type => ITEM_TYPE;
    public float Damage => _damage;
    public float StaminaCost => _staminaCost;
    public float Cooldown => _cooldown;
    public float Accuracy => _accuracy;
    public int SocketsAmount => _socketsAmount;


    [SerializeField] private float _damage;
    [SerializeField] private float _staminaCost;
    [SerializeField] private float _cooldown;
    [SerializeField] private float _accuracy;
    [SerializeField] private int _socketsAmount;

    private const ItemType ITEM_TYPE = ItemType.MeleeWeapons;
  
}
