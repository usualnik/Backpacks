using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeCell : MonoBehaviour
{
    private bool _isCheckingForIngridient = false;

    private ItemBehaviour _itemBehaviour;
    private List<ItemBehaviour> _ingridientItemBehaviours = new List<ItemBehaviour>();

    private Recepie _currentRecepie;
    private bool _canBeCombined = false;

    // Флаг на уровне предмета, что сборка уже выполнена
    private static bool _itemCombinationExecuted = false;

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
        // Если сборка для этого предмета уже выполнена - игнорируем все последующие ячейки
        if (_itemCombinationExecuted)
        {
            return;
        }

        // Проверяем, не запущена ли уже сборка другой ячейкой этого же предмета
        if (IsAnyOtherCellCombining())
        {
            return;
        }

        if (_canBeCombined && _currentRecepie != null)
        {

            try
            {
                // Устанавливаем флаг, что сборка началась
                _itemCombinationExecuted = true;

                // Получаем только те ингредиенты, которые нужны для текущего рецепта
                ItemBehaviour[] ingredientsForRecipe = GetIngredientsForCurrentRecipe();
                _itemBehaviour.CombineItemWithIngridient(ingredientsForRecipe, _currentRecepie.RecipeResult);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);  
                _itemCombinationExecuted = false;
            }
        }

    }

    private bool IsAnyOtherCellCombining()
    {
        // Получаем все RecipeCell на этом предмете
        RecipeCell[] allCells = _itemBehaviour.GetComponentsInChildren<RecipeCell>();

        // Находим все ячейки, которые могут собрать рецепт (включая текущую)
        var combinableCells = allCells
            .Where(cell => cell._canBeCombined && cell._currentRecepie != null)
            .ToList();

        // Если только одна ячейка может собрать рецепт - это наша, и мы её пропускаем дальше
        if (combinableCells.Count == 1 && combinableCells[0] == this)
        {
            return false;
        }

        // Если несколько ячеек могут собрать рецепт, выбираем первую по индексу
        if (combinableCells.Count > 1)
        {
            // Сортируем ячейки по имени или индексу для детерминированного выбора
            var sortedCells = combinableCells
                .OrderBy(cell => cell.name)
                .ThenBy(cell => cell.GetInstanceID())
                .ToList();

            // Если первая ячейка в отсортированном списке - это мы, то собираем
            if (sortedCells[0] == this)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return false;
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
            // Сбрасываем флаг сборки, когда предмет больше не в состоянии проверки
            _itemCombinationExecuted = false;
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