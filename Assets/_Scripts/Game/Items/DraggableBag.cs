using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableBag : MonoBehaviour, IBeginDragHandler,
    IDragHandler, IEndDragHandler
{
    [SerializeField] protected Image _image;
    [SerializeField] private BagCell[] _bagCells;

    protected bool _isDragging;
    protected Rigidbody2D _rb;
    protected Collider2D _collider;
    protected bool _canRotate;
    protected Canvas _canvas;

    private ItemBehaviour _itemBehaviour;
    private InventoryCell[] _targetInventoryCells;

    private int _neededSlotsToBePlaced;
    private int _currentSlotsToBePlaced = 0;
    private bool _isPlacedInInventory = false;

    private void Awake()
    {
        _itemBehaviour = GetComponent<ItemBehaviour>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _targetInventoryCells = new InventoryCell[_bagCells.Length];

        _neededSlotsToBePlaced = _bagCells.Length;
    }

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isDragging) return;
        
        SelectedItemManager.Instance.SetCurrentSelectedItem(_itemBehaviour);


        //// Если предмет уже в инвентаре - освобождаем ячейки при начале перетаскивания
        //if (_isPlacedInInventory)
        //{
        //    ReleaseInventoryCells();
        //}

        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        _image.raycastTarget = false;
        _isDragging = true;
        _canRotate = true;
        _collider.enabled = true;

        transform.SetParent(_canvas.transform, false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        CheckPutInInventory();

        SelectedItemManager.Instance.SetCurrentSelectedItem(null);

        _image.raycastTarget = true;
        _canRotate = false;
        _isDragging = false;
    }

    private void Update()
    {
        if (!_canRotate) return;

        HandleRotation();

        if (_isDragging)
        {
            CheckCanBePlaced();
        }
    }


    private void HandleRotation()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
            {
                transform.Rotate(0, 0, -90);
            }
            else
            {
                transform.Rotate(0, 0, 90);
            }
        }
    }

    private void CheckPutInInventory()
    {
        if (_currentSlotsToBePlaced == _neededSlotsToBePlaced)
        {
            // Сумку можно разместить - привязываем к ячейкам сумки
            PlaceBagInInventoryCells();
            _itemBehaviour.SetItemState(ItemBehaviour.ItemState.Inventory);
            Debug.Log("Bag placed successfully!");
        }
        else
        {
            // Сумку нельзя разместить - возвращаем физику
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _isPlacedInInventory = false;
            _itemBehaviour.SetItemState(ItemBehaviour.ItemState.FreeFall);
            Debug.Log($"Bag cannot be placed: {_currentSlotsToBePlaced}/{_neededSlotsToBePlaced}");
        }
    }

    private void CheckCanBePlaced()
    {
        _currentSlotsToBePlaced = 0;

        for (int i = 0; i < _bagCells.Length; i++)
        {
            if (_bagCells[i].CanBePlaced)
            {
                _currentSlotsToBePlaced++;
                _targetInventoryCells[i] = _bagCells[i].CurrentInventoryCell;
            }
            else
            {
                _targetInventoryCells[i] = null;
            }
        }

        // Визуальная обратная связь
        if (_currentSlotsToBePlaced == _neededSlotsToBePlaced)
        {
            _image.color = new Color(0, 1, 0, 0.5f);
        }
        else
        {
            _image.color = new Color(1, 0, 0, 0.5f);
        }
    }

    private void PlaceBagInInventoryCells()
    {
        if (_targetInventoryCells[0] == null) return;

        // Вычисляем центр всех целевых ячеек
        Vector2 centerPosition = CalculateCenterPosition();

        // Устанавливаем позицию предмета в центр
        transform.position = centerPosition;

        // Делаем предмет кинематическим и выключаем коллайдер
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _collider.enabled = false;

        // Можно также привязать к родительской ячейке
        transform.SetParent(_targetInventoryCells[0].transform.parent, true);

        // Помечаем ячейки как занятые
        foreach (var inventoryCell in _targetInventoryCells)
        {
            if (inventoryCell != null)
            {
                inventoryCell.SetOccupied(true, this);
            }
        }

        _isPlacedInInventory = true;

        // Сбрасываем цвет
        ResetColor();
    }

    // Метод для освобождения ячеек сумки
    private void ReleaseInventoryCells()
    {
        foreach (var inventoryCell in _targetInventoryCells)
        {
            if (inventoryCell != null)
            {
                inventoryCell.SetOccupied(false, null);
            }
        }

        _isPlacedInInventory = false;

        // Очищаем массив целевых ячеек
        for (int i = 0; i < _targetInventoryCells.Length; i++)
        {
           _targetInventoryCells[i] = null;
        }
    }

    private Vector2 CalculateCenterPosition()
    {
        Vector2 sum = Vector2.zero;
        int count = 0;

        foreach (var inventoryCell in _targetInventoryCells)
        {
            if (inventoryCell != null)
            {
                sum += (Vector2)inventoryCell.transform.position;
                count++;
            }
        }

        return count > 0 ? sum / count : transform.position;
    }

    public void ResetColor()
    {
        _image.color = Color.white;
    }

    public void ForceReleaseFromInventory()
    {
        if (_isPlacedInInventory)
        {
            ReleaseInventoryCells();
            _collider.enabled = true;
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

   
}
