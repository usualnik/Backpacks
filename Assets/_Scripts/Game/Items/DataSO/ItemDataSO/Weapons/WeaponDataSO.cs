using UnityEngine;

public abstract class WeaponDataSO : ItemDataSO
{
    public float DamageMin => _damageMin;
    public float DamageMax => _damageMax;

    public float StaminaCost => _staminaCost;
    public float Cooldown => _cooldown;
    public float Accuracy => _accuracy;
    public int SocketsAmount => _socketsAmount;


    [SerializeField] protected float _damageMin;
    [SerializeField] protected float _damageMax;
    [SerializeField] protected float _staminaCost;
    [SerializeField] protected float _cooldown;
    [SerializeField] protected float _accuracy;

    [SerializeField] protected int _socketsAmount;
    public virtual void PerformWeaponAction(ItemBehaviour.Target target, WeaponBehaviour performedItem)
    {

    }

}
