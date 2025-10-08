using System;
using UnityEngine;

public abstract class Character : MonoBehaviour, IDamageable
{
    [System.Serializable]
    public class CharacterStats
    {
        public int GoldAmount = 0;
        public float Health = 0f;
        public float HealthMax = 0f;
        public float Stamina = 0f;        
        public string RankName = string.Empty;
    }

    public event Action<CharacterStats> OnCharacterStatsChanged;
    public event Action OnCharacterDeath;

    [SerializeField] protected string _nickname = string.Empty;
    [SerializeField] protected string _className = string.Empty;
    [SerializeField] protected bool _isDead = false;
    [SerializeField] protected CharacterStats _stats = new CharacterStats();

    public void TakeDamage(float damage)
    {
        if (_stats.Health - damage > 0)
        {
            _stats.Health -= damage;
            OnCharacterStatsChanged?.Invoke(_stats);
        }
        else
        {
            OnCharacterDeath?.Invoke();
        }
       
    }
    public string NickName => _nickname;
    public string ClassName => _className;
    public CharacterStats Stats => _stats;
    public bool IsDead => _isDead;
}