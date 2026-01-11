using UnityEngine;

public class BagPreset : MonoBehaviour
{
    public ItemPreset[] PresetedItems => _presetedItems;
    [SerializeField] private ItemPreset[] _presetedItems;
}
