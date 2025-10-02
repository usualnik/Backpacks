using UnityEngine;

public abstract class Character : MonoBehaviour, IDamageable
{
    [System.Serializable]
    public class CharacterStats
    {
        public int GoldAmount = 0;
        public float Health = 0f;
        public float Stamina = 0f;        
        public string RankName = string.Empty;
    }

   
    public string NickName => _nickname;
    public string ClassName => _className;
    public CharacterStats Stats => _stats;

  
    [SerializeField] protected string _nickname = string.Empty;
    [SerializeField] protected string _className = string.Empty;
    [SerializeField] protected CharacterStats _stats = new CharacterStats();

    public void TakeDamage(float damage)
    {        
        
    }
}