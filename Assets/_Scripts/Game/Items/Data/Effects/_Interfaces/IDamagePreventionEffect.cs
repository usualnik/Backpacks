public interface IDamagePreventionEffect
{
    bool ShouldPreventDamage();
    float GetPreventedDamageAmount();
}