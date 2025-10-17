using UnityEngine;
using UnityEngine.UI;

public class BagCell : MonoBehaviour
{
    public bool IsOccupied { get; private set; }
    public DraggableItem OccupyingItem { get; private set; }

    public void SetOccupied(bool occupied, DraggableItem item)
    {
        IsOccupied = occupied;
        OccupyingItem = item;

        // ¬изуальна€ обратна€ св€зь
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.color = occupied ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }

    // јвтоматически освобождаем €чейку если предмет был уничтожен
    private void OnDestroy()
    {
        if (IsOccupied && OccupyingItem != null)
        {
            SetOccupied(false, null);
        }
    }
}