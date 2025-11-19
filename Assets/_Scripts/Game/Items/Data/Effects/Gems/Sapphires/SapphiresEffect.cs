using UnityEngine;

public class SapphiresEffect : MonoBehaviour, IItemEffect
{
    private DraggableGem _draggableGem;

    private void Awake()
    {
        _draggableGem = GetComponent<DraggableGem>();

    }
    private void Start()
    {
        _draggableGem.OnGemPlacedInItem += DraggableGem_OnGemPlacedInItem;
        _draggableGem.OnGemRemovedFromItem += DraggableGem_OnGemRemovedFromItem;
    }


    private void OnDestroy()
    {
        _draggableGem.OnGemPlacedInItem -= DraggableGem_OnGemPlacedInItem;
        _draggableGem.OnGemRemovedFromItem -= DraggableGem_OnGemRemovedFromItem;

    }
    private void DraggableGem_OnGemRemovedFromItem(ItemBehaviour itemWithSocket)
    {
        ItemDataSO itemData = itemWithSocket.ItemData;

        switch (itemData.Type)
        {
            case ItemDataSO.ItemType.MeleeWeapons | ItemDataSO.ItemType.RangedWeapons:
                RemoveWeaponEffect();
                break;
            case ItemDataSO.ItemType.Bags:
                RemoveBagEffect();
                break;
            default:
                RemoveArmorOrOtherEffect();
                break;
        }
    }

    private void DraggableGem_OnGemPlacedInItem(ItemBehaviour itemWithSocket)
    {
        ItemDataSO itemData = itemWithSocket.ItemData;


        switch (itemData.Type)
        {
            case ItemDataSO.ItemType.MeleeWeapons | ItemDataSO.ItemType.RangedWeapons:
                ApplyWeaponEffect();
                break;
            case ItemDataSO.ItemType.Bags:
                ApplyBagEffect();
                break;
            default:
                ApplyArmorOrOtherEffect();
                break;
        }
    }

    #region Weapons
    private void ApplyWeaponEffect()
    {

    }

    private void RemoveWeaponEffect()
    {

    }

    #endregion

    #region Bags
    private void ApplyBagEffect()
    {

    }
    private void RemoveBagEffect()
    {

    }
    #endregion

    #region Armor + other
    private void ApplyArmorOrOtherEffect()
    {

    }
    private void RemoveArmorOrOtherEffect()
    {

    }
    #endregion


    #region Interface
    public void ApplyEffect(ItemBehaviour item, Character sourceCharacter, Character targetCharacter)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveEffect()
    {
        throw new System.NotImplementedException();
    }
    #endregion

}


