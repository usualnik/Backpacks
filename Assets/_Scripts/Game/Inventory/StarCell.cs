using UnityEngine;
using UnityEngine.UI;

public class StarCell : MonoBehaviour
{
    [SerializeField] private Sprite _starEmpty;
    [SerializeField] private Sprite _starFilled;

    private Image _starImage;
    private OnHoverItem _onHoverItem;
    private bool _isFilled = false;

    private ItemBehaviour _itemBehaviour;

    private void Awake()
    {
        _starImage = GetComponent<Image>();
        _starImage.enabled = false;
        _starImage.sprite = _starEmpty;
    }

    private void Start()
    {
        _onHoverItem = GetComponentInParent<OnHoverItem>();
        _itemBehaviour = GetComponentInParent<ItemBehaviour>();

        _onHoverItem.OnHover += OnHoverItem_OnHover;
        _onHoverItem.OnHoverExit += OnHoverItem_OnHoverExit;
    }

    private void OnDestroy()
    {
        _onHoverItem.OnHover -= OnHoverItem_OnHover;
        _onHoverItem.OnHoverExit -= OnHoverItem_OnHoverExit;
    }

    private void OnHoverItem_OnHover()
    {
        ShowStars();
    }

    private void OnHoverItem_OnHoverExit()
    {
        HideStars();
    }

    private void ShowStars()
    {
        _starImage.enabled = true;
    }

    private void HideStars()
    {
        _starImage.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ItemBehaviour otherItem)
            && _itemBehaviour.ItemData.StarEffect.Target.HasFlag(otherItem.ItemData.Type))
        {
            _starImage.sprite = _starFilled;
            _isFilled = true;

            _itemBehaviour.ItemData.StarEffect.ApplyStarEffect(otherItem);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ItemBehaviour otherItem)
            && _itemBehaviour.ItemData.StarEffect.Target.HasFlag(otherItem.ItemData.Type))
        {
            _starImage.sprite = _starEmpty;
            _isFilled = false;

            _itemBehaviour.ItemData.StarEffect.RemoveStarEffect(otherItem);

        }
    }

    public bool IsFilled => _isFilled;
}