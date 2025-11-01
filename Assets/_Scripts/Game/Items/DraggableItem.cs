using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IDraggable
{
    [SerializeField] protected Image _image;
    [SerializeField] private ItemCell[] _itemCells;
    [SerializeField] private bool _isCanBeSelled = false;

    protected bool _isDragging;
    protected Rigidbody2D _rb;
    protected Collider2D _collider;
    protected bool _canRotate;
    protected Canvas _canvas;

    private ItemBehaviour _itemBehaviour;
    private BagCell[] _targetBagCells;

    private int _neededSlotsToBePlaced;
    private int _currentSlotsToBePlaced = 0;
    private bool _isPlacedInBag = false;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Transform _originalParent;

    private void Awake()
    {
        _itemBehaviour = GetComponent<ItemBehaviour>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _targetBagCells = new BagCell[_itemCells.Length];

        _neededSlotsToBePlaced = _itemCells.Length;

        ValidateComponents();
    }

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("Canvas not found in parent hierarchy!", this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isDragging) return;

        // Сохраняем оригинальные трансформы перед началом перетаскивания
        _originalPosition = transform.position;
        _originalRotation = transform.rotation;
        _originalParent = transform.parent;

        if (SelectedItemManager.Instance != null)
        {
            SelectedItemManager.Instance.SetCurrentSelectedItem(_itemBehaviour);
            _itemBehaviour.SetItemState(ItemBehaviour.ItemState.Dragging);
        }
        else
        {
            Debug.LogWarning("SelectedItemManager instance not found!");
        }

        // Если предмет уже в сумке - освобождаем ячейки при начале перетаскивания
        if (_isPlacedInBag)
        {
            ReleaseBagCells();
        }

        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        _image.raycastTarget = false;
        _isDragging = true;
        _canRotate = true;
        _collider.enabled = true;

        if (_canvas != null)
        {
            transform.SetParent(_canvas.transform, true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        CheckPutInSlot();

        if (_isCanBeSelled && PlayerCharacter.Instance != null)
            PlayerCharacter.Instance.SellItem();

        if (SelectedItemManager.Instance != null)
            SelectedItemManager.Instance.SetCurrentSelectedItem(null);

        _image.raycastTarget = true;
        _canRotate = false;
        _isDragging = false;
    }

    private void Update()
    {
        if (!_canRotate) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            RotateItem(scroll > 0 ? 90 : -90);
        }

        if (_isDragging)
        {
            CheckCanBePlaced();
        }
    }

    private void RotateItem(float angle)
    {
        transform.Rotate(0, 0, angle);

        // Опционально: снэп к ближайшим 90 градусам
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.z = Mathf.Round(currentRotation.z / 90) * 90;
        transform.eulerAngles = currentRotation;
    }

    private void CheckPutInSlot()
    {
        bool canAfford = PlayerCharacter.Instance != null &&
                        PlayerCharacter.Instance.HasMoneyToBuyItem(_itemBehaviour.GetItemPrice());

        bool canPlace = _currentSlotsToBePlaced == _neededSlotsToBePlaced;

        if (canPlace && canAfford)
        {
            // Предмет можно разместить - привязываем к ячейкам сумки
            PlaceItemInBagCells();
            _itemBehaviour.SetItemState(ItemBehaviour.ItemState.Inventory);
        }
        else
        {

            if (_itemBehaviour.PreviousState.HasFlag(ItemBehaviour.ItemState.Store))               
            {
                ReturnIfNotPlaced();
            }
            else
            {
                FreeFall();
            }
        }
    }


    private void CheckCanBePlaced()
    {
        _currentSlotsToBePlaced = 0;

        for (int i = 0; i < _itemCells.Length; i++)
        {
            if (_itemCells[i].CanBePlaced)
            {
                _currentSlotsToBePlaced++;
                // Сохраняем ссылку на ячейку сумки для этого ItemCell
                _targetBagCells[i] = _itemCells[i].CurrentBagCell;
            }
            else
            {
                _targetBagCells[i] = null;
            }
        }

        // Визуальная обратная связь
        if (_currentSlotsToBePlaced == _neededSlotsToBePlaced)
        {
            _image.color = new Color(0, 1, 0, 0.7f);
        }
        else
        {
            _image.color = new Color(1, 0, 0, 0.7f);
        }
    }

    private void PlaceItemInBagCells()
    {
        if (_targetBagCells[0] == null) return;

        // Вычисляем центр всех целевых ячеек
        Vector2 centerPosition = CalculateCenterPosition();

        // Устанавливаем позицию предмета в центр
        transform.position = centerPosition;

        // Делаем предмет кинематическим и выключаем коллайдер
        _rb.bodyType = RigidbodyType2D.Kinematic;
        //_collider.enabled = false;

        // Привязываем к родительской ячейке
        transform.SetParent(_targetBagCells[0].transform.parent, true);

        // Помечаем ячейки как занятые
        foreach (var bagCell in _targetBagCells)
        {
            if (bagCell != null)
            {
                bagCell.SetOccupied(true, this);
            }
        }

        _isPlacedInBag = true;

        // Сбрасываем цвет
        ResetColor();
    }

    // Метод для освобождения ячеек сумки
    private void ReleaseBagCells()
    {
        foreach (var bagCell in _targetBagCells)
        {
            if (bagCell != null)
            {
                bagCell.SetOccupied(false, null);
            }
        }

        _isPlacedInBag = false;

        // Очищаем массив целевых ячеек
        for (int i = 0; i < _targetBagCells.Length; i++)
        {
            _targetBagCells[i] = null;
        }
    }

    private Vector2 CalculateCenterPosition()
    {
        Vector2 sum = Vector2.zero;
        int count = 0;

        foreach (var bagCell in _targetBagCells)
        {
            if (bagCell != null)
            {
                sum += (Vector2)bagCell.transform.position;
                count++;
            }
        }

        return count > 0 ? sum / count : transform.position;
    }

    public void ResetColor()
    {
        _image.color = Color.white;
    }

    public void ForceReleaseFromBag()
    {
        if (_isPlacedInBag)
        {
            ReleaseBagCells();
            FreeFall();
        }
    }

    public void SetCanBeSelled()
    {
        _isCanBeSelled = !_isCanBeSelled;
    }

    public void ReturnIfNotPlaced()
    {
        // Восстанавливаем оригинальную позицию и rotation
        transform.position = _originalPosition;
        transform.rotation = _originalRotation;

        // Восстанавливаем оригинального родителя
        if (_originalParent != null)
        {
            transform.SetParent(_originalParent, true);
        }
               
        ResetColor();
    }

    private void ValidateComponents()
    {
        if (_image == null) _image = GetComponent<Image>();
        if (_itemBehaviour == null) _itemBehaviour = GetComponent<ItemBehaviour>();
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_collider == null) _collider = GetComponent<Collider2D>();

        if (_itemCells == null || _itemCells.Length == 0)
        {
            Debug.LogError("ItemCells array is not set up!", this);
        }
    }

    private void OnDisable()
    {
        // Clean up when object is disabled
        if (_isDragging)
        {
            OnEndDrag(new PointerEventData(EventSystem.current));
        }
    }

    private void OnDestroy()
    {
        // Release any occupied cells when destroyed
        if (_isPlacedInBag)
        {
            ReleaseBagCells();
        }
    }

    public void FreeFall()
    {
        _collider.enabled = true;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _itemBehaviour.SetItemState(ItemBehaviour.ItemState.FreeFall);


    }
}