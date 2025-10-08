using UnityEngine;
public class EnemyCharacter : Character
{
    public static EnemyCharacter Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("More than one instance of player character");

        _stats.Health = _stats.HealthMax;
    }

}
