
public interface IDamageable
{
    public void TakeDamage(float damage, ItemDataSO.ItemType weaponType);
}

public interface IStaminable
{
    public void UseStamina(float amount);
}
