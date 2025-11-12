using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

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

    private void Awake()
    {
        _starImage = GetComponent<Image>();
        _starImage.enabled = false;
        _starImage.sprite = _starEmpty;
    }

    private void Start()
    {
        _onHoverItem = GetComponentInParent<OnHoverItem>();
        _starEffect = GetComponentInParent<IStarEffect>();
        _item = GetComponentInParent<ItemBehaviour>();

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
            && otherItem.ItemData.Type.HasFlag(_starEffectTarget))
        {
            ApplyStarEffect(otherItem);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ItemBehaviour otherItem)
            && otherItem.ItemData.Type.HasFlag(_starEffectTarget))
        {
           RemoveStarEffect(otherItem);
        }
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


