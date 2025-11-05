using UnityEngine;

public class RecipeCell : MonoBehaviour
{
    [SerializeField] private ItemDataSO[] _ingridients;
    [SerializeField] private ItemDataSO _recipeResult;

    private bool _isCheckingForIngridient = false;
    private ItemBehaviour _itemBehaviour;

    private ItemBehaviour _ingridientItemBehaviour;

    private bool _canBeCombined;

    private void Awake()
    {
        _itemBehaviour = GetComponentInParent<ItemBehaviour>();
    }
    private void Start()
    {
        _itemBehaviour.OnItemStateChanged += ItemBehaviour_OnItemStateChanged;

        _ingridients = _itemBehaviour.ItemData.RecipeIngridients;
        _recipeResult = _itemBehaviour.ItemData.RecipeResult;

        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;

    }


    private void OnDestroy()
    {
        _itemBehaviour.OnItemStateChanged -= ItemBehaviour_OnItemStateChanged;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;

    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {
        if (_canBeCombined)
        {
            _itemBehaviour.CombineWithIngridient(_ingridientItemBehaviour, _recipeResult);
        }
    }
    private void ItemBehaviour_OnItemStateChanged(ItemBehaviour.ItemState prevState, ItemBehaviour.ItemState currentState)
    {
        if (currentState.HasFlag(ItemBehaviour.ItemState.Inventory) 
            || currentState.HasFlag(ItemBehaviour.ItemState.Dragging))
        {
            _isCheckingForIngridient = true;
        }
        else
        {
            _isCheckingForIngridient = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isCheckingForIngridient)
            return;

        if (collision.TryGetComponent(out IngridientCell ingridientCell))
        {
            //HACK: Проверяется только первый элемент списка ингридиентов для комбинации

            _ingridientItemBehaviour = ingridientCell.GetComponentInParent<ItemBehaviour>();
            _canBeCombined = _ingridientItemBehaviour.ItemData == _ingridients[0] ? true : false;     
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_isCheckingForIngridient)
            return;

        if (collision.TryGetComponent(out IngridientCell ingridientCell))
        {
            _canBeCombined = false;
            _ingridientItemBehaviour = null;

        }

    }
}
