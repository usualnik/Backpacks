using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
    private bool _isDragging;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private Image _image;
    private bool _canRotate;
    private Canvas _canvas;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _image = GetComponent<Image>();
    }
    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {

        if (_isDragging) return;

        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        _image.raycastTarget = false;
        _isDragging = true;
        _canRotate = true;
               
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

        CheckPutInSlot(eventData);

        _image.raycastTarget = true;
        _canRotate = false;
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

    }

    private void CheckPutInSlot(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null &&
       eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out InventoryCell cell))
        {
            transform.position = cell.transform.position;
            _isDragging = false;            
        }
        else
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _isDragging = false;
        }
    }


}