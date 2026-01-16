using System;
using UnityEngine;

public class DraggableGem : DraggableItem
{
    public event Action<ItemBehaviour> OnGemPlacedInItem;
    public event Action<ItemBehaviour> OnGemRemovedFromItem;


    private GemSocket[] _targetGemCells;
    private bool _isTryingToPutInSocket = false;

    private void Start()
    {
        _targetGemCells = new GemSocket[_itemCells.Length];
    }

    protected override void CheckCanBePlaced()
    {
        _currentSlotsToBePlaced = 0;
        _isTryingToPutInSocket = false;

        bool canPlaceInSocket = true;

        for (int i = 0; i < _itemCells.Length; i++)
        {
            var gemCell = _itemCells[i] as GemCell;

          
            if (gemCell != null && gemCell.CurrentGemSocket != null)
            {
                _currentSlotsToBePlaced++;
                _targetGemCells[i] = gemCell.CurrentGemSocket;
                _targetBagCells[i] = null;
            }
            else
            {
                canPlaceInSocket = false;
                break;
            }
        }


        if (canPlaceInSocket && _currentSlotsToBePlaced == _neededSlotsToBePlaced)
        {
            _isTryingToPutInSocket = true;
        }
        else
        {
            _currentSlotsToBePlaced = 0;

            for (int i = 0; i < _itemCells.Length; i++)
            {

                if (_itemCells[i].CanBePlaced && _itemCells[i].CurrentBagCell != null)
                {
                    _currentSlotsToBePlaced++;
                    _targetBagCells[i] = _itemCells[i].CurrentBagCell;
                    _targetGemCells[i] = null;
                }
                else
                {
                    _targetBagCells[i] = null;
                    _targetGemCells[i] = null;
                }
            }
        }

        bool canPlace = _currentSlotsToBePlaced == _neededSlotsToBePlaced;
        _image.color = canPlace ? new Color(0, 1, 0, 0.7f) : new Color(1, 0, 0, 0.7f);
    }

    protected override void PlaceItemInBagCells()
    {       

        if (_isTryingToPutInSocket && _targetGemCells[0] != null)
        {
            transform.position = _targetGemCells[0].transform.position;
            _rb.bodyType = RigidbodyType2D.Kinematic;

            foreach (var gemSocket in _targetGemCells)
            {
                if (gemSocket != null)
                {
                    gemSocket.SetOccupied(true, this);
                    OnGemPlacedInItem?.Invoke(gemSocket.Item);
                    PlayerCharacter.Instance.SpendMoney(_itemBehaviour.GetItemPrice());

                }
            }

            transform.SetParent(_targetGemCells[0].transform, true);
            _isPlaced = true;
        }
        else if (_targetBagCells[0] != null)
        {
            OnGemPlacedInItem?.Invoke(_targetBagCells[0].BagItem);
            base.PlaceItemInBagCells();
            PlayerCharacter.Instance.SpendMoney(_itemBehaviour.GetItemPrice());
        }
    
        ResetColor();
    }
    protected override void ReleaseBagCells()
    {
        // Освобождаем ячейки сумки
        foreach (var bagCell in _targetBagCells)
        {
            if (bagCell != null)
            {
                bagCell.SetOccupied(false, this);
                OnGemRemovedFromItem?.Invoke(bagCell.BagItem);
            }
        }

        // Освобождаем гнезда самоцветов
        foreach (var gemSocket in _targetGemCells)
        {
            if (gemSocket != null)
            {
                gemSocket.SetOccupied(false, this);
                OnGemRemovedFromItem.Invoke(gemSocket.Item);
            }
        }

        _isPlaced = false;

        // Очищаем массивы
        for (int i = 0; i < _targetBagCells.Length; i++)
        {
            _targetBagCells[i] = null;
            _targetGemCells[i] = null;
        }

    }
}