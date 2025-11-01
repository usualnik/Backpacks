using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private DescriptionBox _descriptionBox;
    public event Action OnHover;
    public event Action OnHoverExit;

    private void Start()
    {
        _descriptionBox = GetComponentInChildren<DescriptionBox>(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _descriptionBox.gameObject.SetActive(true);
        OnHover?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descriptionBox.gameObject.SetActive(false);
        OnHoverExit?.Invoke();
    }


}
