using UnityEngine;

public class UI_DescriptionCanvas : MonoBehaviour
{
    public static UI_DescriptionCanvas Instance { get; private set; }
    public bool IsAnyDescriptionActive { get; private set; }

    [SerializeField] private int _activeDescriptionsCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            Debug.LogError("More than one instance of UI_DescriptionCanvas");
        }
    }

    private void Start()
    {
        OnHoverItem.OnAnyItemHovered += OnHoverItem_OnAnyItemHovered;
    }

    private void OnDestroy()
    {
        OnHoverItem.OnAnyItemHovered -= OnHoverItem_OnAnyItemHovered;
    }

    private void OnHoverItem_OnAnyItemHovered(bool isHovered)
    {
        if (isHovered)
        {
            _activeDescriptionsCount++;
        }
        else
        {
            _activeDescriptionsCount--;
        }

        _activeDescriptionsCount = Mathf.Max(0, _activeDescriptionsCount);

        IsAnyDescriptionActive = _activeDescriptionsCount > 0;
    }
}