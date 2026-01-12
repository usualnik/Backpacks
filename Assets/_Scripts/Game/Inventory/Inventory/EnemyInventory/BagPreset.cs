using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class BagPreset : MonoBehaviour
{
    public List<ItemPreset> PresetedItems => _presetedItems;
    [SerializeField] private List<ItemPreset> _presetedItems;

    private void Awake()
    {

    }
    public void InitPresetedItemsInBag()
    {
        ItemBehaviour[] items = GetComponentsInChildren<ItemBehaviour>();

        _presetedItems = new List<ItemPreset>();


        // Стартуем с 1, чтобы скинпнуть саму сумку
        for (int i = 1; i < items.Length; i++)
        {
            ItemPreset itemPreset = items[i].AddComponent<ItemPreset>();

            _presetedItems.Add(itemPreset);
        }    

    }
    public void ClearPresetList()
    {
        _presetedItems.Clear();
    }

}
