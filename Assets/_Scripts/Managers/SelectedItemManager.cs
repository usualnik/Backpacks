using System;
using UnityEngine;

public class SelectedItemManager : MonoBehaviour
{
    public event Action<ItemBehaviour> OnItemSelected;

    public static SelectedItemManager Instance { get; private set; }

    [SerializeField] private ItemBehaviour _currentSelectedItem;
     
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one instance of ItemsManager");
        }
    }

    public void SetCurrentSelectedItem(ItemBehaviour item)
    {
        _currentSelectedItem = item;
        OnItemSelected?.Invoke(item);
    }
}
