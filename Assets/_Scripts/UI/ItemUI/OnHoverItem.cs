using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<bool> OnAnyItemHovered; 
    public event Action OnHover;
    public event Action OnHoverExit;

    private DescriptionBox _descriptionBox;
    private UI_DescriptionCanvas _descriptionCanvas;
    private Transform _originalTransform;

    private void Start()
    {
        _descriptionBox = GetComponentInChildren<DescriptionBox>(true);
        _descriptionCanvas = FindAnyObjectByType<UI_DescriptionCanvas>();
        _originalTransform = GetComponentInParent<ItemBehaviour>().transform;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_descriptionCanvas == null) return;
        if (UI_DescriptionCanvas.Instance.IsAnyDescriptionActive) return;

        _descriptionBox.gameObject.SetActive(true);
        _descriptionBox.transform.SetParent(_descriptionCanvas.transform, true);

        OnAnyItemHovered?.Invoke(true);
        OnHover?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_descriptionCanvas == null) return;

        _descriptionBox.gameObject.SetActive(false);
        _descriptionBox.transform.SetParent(_originalTransform, true);

        OnAnyItemHovered?.Invoke(false); 
        OnHoverExit?.Invoke();
    }
}