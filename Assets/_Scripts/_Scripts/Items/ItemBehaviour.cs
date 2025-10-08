using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    [SerializeField] private ItemDataSO _itemData;

    public enum Target
    {
        Player,
        Enemy
    }

    [SerializeField] private Target _target;

    private void Start()
    {
        CombatManager.Instance.OnCombatStarted += CombatManager_OnCombatStarted;
    }
    private void OnDestroy()
    {
        CombatManager.Instance.OnCombatStarted -= CombatManager_OnCombatStarted;

    }

    private void CombatManager_OnCombatStarted()
    {
        _itemData.PerformAction(_target);
    }
}
