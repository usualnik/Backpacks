using UnityEngine;

public class GemCell : ItemCell
{
    public GemSocket CurrentGemSocket { get; private set; }

    protected override void HandleTriggerEnter(Collider2D collision)
    {
        if (ShouldIgnoreCollision(collision)) return;
        CheckPlacement(collision);
    }

    protected override void HandleTriggerExit(Collider2D collision)
    {
        if (ShouldIgnoreCollision(collision)) return;

        if (collision.TryGetComponent(out BagCell _) || collision.TryGetComponent(out GemSocket _))
        {
            ResetPlacement();
        }
    }

    protected override void HandleTriggerStay(Collider2D collision)
    {
        if (ShouldIgnoreCollision(collision)) return;
        CheckPlacement(collision);
    }

    private bool ShouldIgnoreCollision(Collider2D collision)
    {
        // Игнорируем InventoryCell и любые другие коллайдеры, кроме BagCell и GemSocket
        if (collision.TryGetComponent(out InventoryCell _)) return true;
        if (!collision.TryGetComponent(out BagCell _) && !collision.TryGetComponent(out GemSocket _)) return true;

        return false;
    }

    private void CheckPlacement(Collider2D collision)
    {
        // В первую очередь проверяем GemSocket
        if (collision.TryGetComponent(out GemSocket gemSocket))
        {
            if (!gemSocket.IsOccupied || gemSocket.OccupyingGem == GetComponentInParent<DraggableGem>())
            {
                CanBePlaced = true;
                CurrentGemSocket = gemSocket;
                CurrentBagCell = null;
                UpdateVisuals(true);
                return;
            }
        }

        // Только если нет подходящего GemSocket, проверяем BagCell
        if (collision.TryGetComponent(out BagCell bagCell))
        {
            if (!bagCell.IsOccupied || bagCell.OccupyingItem == GetComponentInParent<DraggableItem>())
            {
                CanBePlaced = true;
                CurrentBagCell = bagCell;
                CurrentGemSocket = null;
                UpdateVisuals(true);
                return;
            }
        }

        ResetPlacement();
    }

    private void ResetPlacement()
    {
        CanBePlaced = false;
        CurrentBagCell = null;
        CurrentGemSocket = null;
        UpdateVisuals(false);
    }

    private void UpdateVisuals(bool canPlace)
    {
        _itemCellImage.color = canPlace ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
    }
}