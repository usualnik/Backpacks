
public class FoodStarCell : StarCell
{
    protected override void ApplyStarEffect(ItemBehaviour otherItem)
    {
        if (_item.ItemData.ItemName != otherItem.ItemData.ItemName)
        {
            _isFilled = true;
            _starImage.sprite = _starFilled;

            _starEffect?.ApplyStarEffect(_item, otherItem, this);
        }

    }

    protected override void RemoveStarEffect(ItemBehaviour otherItem)
    {
        if (_item.ItemData.ItemName != otherItem.ItemData.ItemName)
        {
            _isFilled = false;
            _starImage.sprite = _starEmpty;
            _starEffect?.RemoveStarEffect(_item, otherItem, this);
        }

    }
}