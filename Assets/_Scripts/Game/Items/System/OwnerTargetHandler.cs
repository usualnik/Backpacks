using UnityEngine;

public class OwnerTargetHandler : MonoBehaviour
{
        
    public enum Target
    {
        None,
        Player,
        Enemy
    }


    public enum Owner
    {        
        Player,
        Enemy
    }

    [Header("Player config")]
    [SerializeField] private Target playerConfigTarget;
    [SerializeField] private Owner playerConfigOwner;

    [Header("Enemy config")]
    [SerializeField] private Target enemyConfigTarget;
    [SerializeField] private Owner enemyConfigOwner;

    [Tooltip("Определяет стоит ли загрузить конфиг игрока или врага")]
    [SerializeField] private bool _isPlayerObject;

    public Target GetTarget()
    {
        if (_isPlayerObject)
        {
            return playerConfigTarget;
        }
        else
        {
            return enemyConfigTarget;
        }
    }
    public Owner GetOwner()
    {
        if (_isPlayerObject)
        {
            return playerConfigOwner;
        }
        else
        {
            return enemyConfigOwner;
        }
    }

    public void SetPlayerItem(bool isPlayerItem)
    {
        _isPlayerObject = isPlayerItem;
    }

}
