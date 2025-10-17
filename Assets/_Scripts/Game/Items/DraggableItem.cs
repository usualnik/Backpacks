using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] protected Image _image;
    [SerializeField] private ItemCell[] _itemCells;

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

    private void Awake()
    {
        _itemBehaviour = GetComponent<ItemBehaviour>();
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _targetBagCells = new BagCell[_itemCells.Length];

        _neededSlotsToBePlaced = _itemCells.Length;
    }

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isDragging) return;

        // ���� ������� ��� � ����� - ����������� ������ ��� ������ ��������������
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

        CheckPutInSlot();

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
            if (scroll > 0)
            {
                transform.Rotate(0, 0, -90);
            }
            else
            {
                transform.Rotate(0, 0, 90);
            }
        }

        if (_isDragging)
        {
            CheckCanBePlaced();
        }
    }

    private void CheckPutInSlot()
    {
        if (_currentSlotsToBePlaced == _neededSlotsToBePlaced)
        {
            // ������� ����� ���������� - ����������� � ������� �����
            PlaceItemInBagCells();
            Debug.Log("Item placed successfully!");
        }
        else
        {
            // ������� ������ ���������� - ���������� ������
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _isPlacedInBag = false;
            Debug.Log($"Item cannot be placed: {_currentSlotsToBePlaced}/{_neededSlotsToBePlaced}");
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
                // ��������� ������ �� ������ ����� ��� ����� ItemCell
                _targetBagCells[i] = _itemCells[i].CurrentBagCell;
            }
            else
            {
                _targetBagCells[i] = null;
            }
        }

        // ���������� �������� �����
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

        // ��������� ����� ���� ������� �����
        Vector2 centerPosition = CalculateCenterPosition();

        // ������������� ������� �������� � �����
        transform.position = centerPosition;

        // ������ ������� �������������� � ��������� ���������
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _collider.enabled = false;

        // ����� ����� ��������� � ������������ ������
        transform.SetParent(_targetBagCells[0].transform.parent, true);

        // �������� ������ ��� �������
        foreach (var bagCell in _targetBagCells)
        {
            if (bagCell != null)
            {
                bagCell.SetOccupied(true, this);
            }
        }

        _isPlacedInBag = true;

        // ���������� ����
        ResetColor();
    }

    // ����� ��� ������������ ����� �����
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

        // ������� ������ ������� �����
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
            _collider.enabled = true;
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}