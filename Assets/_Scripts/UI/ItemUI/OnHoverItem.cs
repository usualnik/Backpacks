using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private DescriptionBox _descriptionBox;

    private void Start()
    {
        _descriptionBox = GetComponentInChildren<DescriptionBox>(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _descriptionBox.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _descriptionBox.gameObject.SetActive(false);
    }


}
