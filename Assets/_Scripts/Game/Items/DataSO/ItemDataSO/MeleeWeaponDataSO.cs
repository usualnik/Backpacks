using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Items/Melee Weapon Data")]
public class MeleeWeaponDataSO : ItemDataSO
{
    public ItemType Type => ITEM_TYPE;
    public float DamageMin => _damageMin;
    public float DamageMax => _damageMax;
    public float StaminaCost => _staminaCost;
    public float Cooldown => _cooldown;
    public float Accuracy => _accuracy;
    public int SocketsAmount => _socketsAmount;


    [SerializeField] private float _damageMin;
    [SerializeField] private float _damageMax;
    [SerializeField] private float _staminaCost;
    [SerializeField] private float _cooldown;
    [SerializeField] private float _accuracy;

    [SerializeField] private int _socketsAmount;

    private const ItemType ITEM_TYPE = ItemType.MeleeWeapons;

    public override void PerformAction(ItemBehaviour.Target target)
    {
        CombatManager.Instance.StartAutoAttack(target,this,_damageMin,_damageMax,_staminaCost,_cooldown,_accuracy);
               
    }
}
