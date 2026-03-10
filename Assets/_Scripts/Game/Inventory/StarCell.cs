using UnityEngine;
using UnityEngine.UI;
using static ItemDataSO;

public class StarCell : MonoBehaviour
{
    [SerializeField] protected ItemDataSO.ItemType _starEffectTarget;

    [Header("System")]
    [SerializeField] protected Sprite _starEmpty;
    [SerializeField] protected Sprite _starFilled;

    protected Image _starImage;
    protected OnHoverItem _onHoverItem;
    protected bool _isFilled = false;
    protected IStarEffect _starEffect;
    protected ItemBehaviour _item;

    private UI_DragItemsCanvas uI_DragItemsCanvas;
    private Transform _originalTransform;

    private void Awake()
    {
        _starImage = GetComponent<Image>();
        _starImage.enabled = false;
        _starImage.sprite = _starEmpty;

        uI_DragItemsCanvas = FindAnyObjectByType<UI_DragItemsCanvas>();
        _originalTransform = GetComponentInParent<ItemBehaviour>().transform;

    }

    private void Start()
    {
        _onHoverItem = GetComponentInParent<OnHoverItem>();
        _starEffect = GetComponentInParent<IStarEffect>();
        _item = GetComponentInParent<ItemBehaviour>();

        if (_onHoverItem != null)
        {
            _onHoverItem.OnHover += OnHoverItem_OnHover;
            _onHoverItem.OnHoverExit += OnHoverItem_OnHoverExit;
        }
    }

    private void OnDestroy()
    {
        if (_onHoverItem != null)
        {
            _onHoverItem.OnHover -= OnHoverItem_OnHover;
            _onHoverItem.OnHoverExit -= OnHoverItem_OnHoverExit;
        }


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

        _starImage.gameObject.transform.SetParent(uI_DragItemsCanvas.transform, true);
    }

    private void HideStars()
    {
        _starImage.enabled = false;

        _starImage.gameObject.transform.SetParent(_originalTransform, true);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ItemBehaviour otherItem))
        {
            if (HasMatchingTypes(otherItem.ItemData.Type))
            {
                ApplyStarEffect(otherItem);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ItemBehaviour otherItem))
        {
            if (HasMatchingTypes(otherItem.ItemData.Type))
            {
                RemoveStarEffect(otherItem);
            }
        }
    }

    private bool HasMatchingTypes(ItemType itemType)
    {
        if (itemType == ItemType.None)
            return false;

        return (_starEffectTarget & itemType) != ItemType.None;
    }

    protected virtual void ApplyStarEffect(ItemBehaviour otherItem)
    {
        _isFilled = true;
        _starImage.sprite = _starFilled;

        _starEffect?.ApplyStarEffect(_item, otherItem, this);
    }

    protected virtual void RemoveStarEffect(ItemBehaviour otherItem)
    {
        _isFilled = false;
        _starImage.sprite = _starEmpty;

        _starEffect?.RemoveStarEffect(_item, otherItem, this);
    }

    public bool IsFilled => _isFilled;
}