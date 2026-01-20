using System.Collections.Generic;
using UnityEngine;

public class BloodHarvesterStarEffect : MonoBehaviour, IStarEffect
{
    private float _vampirismIncreaseMultiplier = 1f;

    private List<ItemBehaviour> _itemsInStar = new List<ItemBehaviour>();

    private ItemBehaviour _bloodHarvester;
    private Character _owner;

    private void Start()
    {
        _bloodHarvester = GetComponent<ItemBehaviour>();
    }
    public void ApplyStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (!_itemsInStar.Contains(targetItem))
        {
            _itemsInStar.Add(targetItem);

            AddVamprirsmIncreceBuff();
        }
    }

    public void RemoveStarEffect(ItemBehaviour sourceItem, ItemBehaviour targetItem, StarCell starCell)
    {
        if (_itemsInStar.Contains(targetItem))
        {
            _itemsInStar.Remove(targetItem);
            RemoveVAmpirismIncreseBuff();

        }
    }

    private void AddVamprirsmIncreceBuff()
    {
        _owner = _bloodHarvester.OwnerCharacter;

        if (_owner != null)
        {
            _owner.AddLifestealMultiplier(_vampirismIncreaseMultiplier);
        }
    }

    private void RemoveVAmpirismIncreseBuff()
    {
        _owner = _bloodHarvester.OwnerCharacter;

        if (_owner != null)
        {
            _owner.AddLifestealMultiplier(-_vampirismIncreaseMultiplier);
        }
    }
 
}
