
public interface IDamageable
{
    public void TakeDamage(float damage, ItemDataSO.ExtraType weaponType);
}

public interface IStaminable
{
    public void UseStamina(float amount);
}
