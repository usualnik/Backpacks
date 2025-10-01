using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    
    public void ApplyLuckBuff(int charges)
    {
        //LuckBuffSO luckBuff = Resources.Load<LuckBuffSO>("Buffs/LuckBuff");
        //playerStats.AddBuff(luckBuff, charges);
    }

    public void ApplyPoisonDebuff(int charges)
    {
       //PoisonDebuffSO poison = Resources.Load<PoisonDebuffSO>("Debuffs/PoisonDebuff");
       // playerStats.AddBuff(poison, charges);
    }
    
    public List<string> GetActiveBuffDescriptions()
    {
        var descriptions = new List<string>();       
        return descriptions;
    }
}