using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeCell : MonoBehaviour
{
    private bool _isCheckingForIngridient = false;

    private ItemBehaviour _itemBehaviour;
    [SerializeField] private List<ItemBehaviour> _ingridientItemBehaviours = new List<ItemBehaviour>();

    [SerializeField]
    private Recepie _currentRecepie;

    [SerializeField]
    private bool _canBeCombined = false;

    private void Awake()
    {
        _itemBehaviour = GetComponentInParent<ItemBehaviour>();
    }

    private void Start()
    {
        _itemBehaviour.OnItemStateChanged += ItemBehaviour_OnItemStateChanged;
        CombatManager.Instance.OnCombatFinished += CombatManager_OnCombatFinished;
    }

    private void OnDestroy()
    {
        _itemBehaviour.OnItemStateChanged -= ItemBehaviour_OnItemStateChanged;
        CombatManager.Instance.OnCombatFinished -= CombatManager_OnCombatFinished;
    }

    private void CombatManager_OnCombatFinished(CombatManager.CombatResult obj)
    {

        if (_canBeCombined && _currentRecepie != null)
        {

            ItemBehaviour[] ingredientsForRecipe = GetIngredientsForCurrentRecipe();
            _itemBehaviour.CombineItemWithIngridient(ingredientsForRecipe, _currentRecepie.RecipeResult);

        }

    }


    private ItemBehaviour[] GetIngredientsForCurrentRecipe()
    {
        if (_currentRecepie == null || _ingridientItemBehaviours.Count == 0)
            return new ItemBehaviour[0];

        List<ItemBehaviour> result = new List<ItemBehaviour>();
        List<ItemDataSO> requiredIngredients = new List<ItemDataSO>(_currentRecepie.RecipeIngridients);

        List<ItemBehaviour> availableIngredients = new List<ItemBehaviour>(_ingridientItemBehaviours);

        foreach (var required in requiredIngredients)
        {
            ItemBehaviour found = availableIngredients
                .FirstOrDefault(i => i.ItemData == required);

            if (found != null)
            {
                result.Add(found);
                availableIngredients.Remove(found);
            }
        }

        return result.ToArray();
    }

    private void ItemBehaviour_OnItemStateChanged(ItemBehaviour.ItemState prevState, ItemBehaviour.ItemState currentState)
    {
        _isCheckingForIngridient = currentState.HasFlag(ItemBehaviour.ItemState.Inventory)
                                    || currentState.HasFlag(ItemBehaviour.ItemState.Dragging);

        if (!_isCheckingForIngridient)
        {
            ClearIngridients();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isCheckingForIngridient)
            return;

        if (collision.TryGetComponent(out IngridientCell ingridientCell))
        {
            ItemBehaviour ingridientBehaviour = ingridientCell.GetComponentInParent<ItemBehaviour>();

            if (ingridientBehaviour != null && !_ingridientItemBehaviours.Contains(ingridientBehaviour))
            {
                _ingridientItemBehaviours.Add(ingridientBehaviour);
                CheckForValidRecipe();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_isCheckingForIngridient)
            return;

        if (collision.TryGetComponent(out IngridientCell ingridientCell))
        {
            ItemBehaviour ingridientBehaviour = ingridientCell.GetComponentInParent<ItemBehaviour>();

            if (ingridientBehaviour != null)
            {
                _ingridientItemBehaviours.Remove(ingridientBehaviour);
                CheckForValidRecipe();
            }
        }
    }

    private void CheckForValidRecipe()
    {
        _canBeCombined = false;
        _currentRecepie = null;

        if (_ingridientItemBehaviours.Count == 0)
            return;

        // Проверяем все рецепты предмета
        foreach (var recipe in _itemBehaviour.ItemData.Recepies)
        {
            if (IsRecipeValid(recipe))
            {
                _canBeCombined = true;
                _currentRecepie = recipe;
                return;
            }
        }
    }

    private bool IsRecipeValid(Recepie recipe)
    {
        if (recipe.RecipeIngridients.Length > _ingridientItemBehaviours.Count)
            return false;

        List<ItemDataSO> requiredIngredients = new List<ItemDataSO>(recipe.RecipeIngridients);
        List<ItemDataSO> availableIngredients = _ingridientItemBehaviours
            .Select(i => i.ItemData)
            .ToList();

        foreach (var required in requiredIngredients)
        {
            if (!availableIngredients.Contains(required))
                return false;

            availableIngredients.Remove(required);
        }

        return true;
    }

    private void ClearIngridients()
    {
        _ingridientItemBehaviours.Clear();
        _canBeCombined = false;
        _currentRecepie = null;
    }
}