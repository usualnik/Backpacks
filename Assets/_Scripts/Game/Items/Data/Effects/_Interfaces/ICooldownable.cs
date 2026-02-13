public interface ICooldownable
{
    float BaseCooldown { get; }
    float CooldownMultiplier { get; set; }
    float CurrentCooldown { get;}      
}
