using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryPanel : MonoBehaviour 
{
    private ItemType _ActiveSubscreen;

    public GameObject _ItemMiniaturePrefab;
    public GameObject _EssenceMiniaturePrefab;
    public Transform _ItemGrid;

    public void ShowEquipment( bool activate )
    {
        if (activate)
        {
            _ActiveSubscreen = ItemType.EQUIPMENT;
            FillInventory();
        }
    }

    public void ShowConsumable(bool activate)
    {
        if (activate)
        {
            _ActiveSubscreen = ItemType.CONSUMABLE;
            FillInventory();
        }
    }

    public void ShowMaterial(bool activate)
    {
        if (activate)
        {
            _ActiveSubscreen = ItemType.MATERIAL;
            FillInventory();
        }
    }

    public void ShowEssence(bool activate)
    {
        if (activate)
        {
            _ActiveSubscreen = ItemType.ESSENCE;
            FillInventory();
        }
    }

    private void OnEnable()
    {
        FillInventory();
        InventoryManager.InventoryChangedEvent += FillInventory;
        InventoryManager.SpecificEssencesChangedEvent += FillInventory;
    }

    private void OnDisable()
    {
        InventoryManager.InventoryChangedEvent -= FillInventory;
        InventoryManager.SpecificEssencesChangedEvent -= FillInventory;
    }

    void FillInventory()
    {
        //Clear inventory grid
        foreach (Transform child in _ItemGrid)
            Destroy(child.gameObject);

        switch (_ActiveSubscreen)
        {
            case ItemType.ESSENCE :
                EssenceInventory essences = InventoryManager.Ref.GetEssenceInventory();

                foreach (EssenceValue essenceValue in essences)
                    Instantiate(_EssenceMiniaturePrefab, _ItemGrid).SendMessage("SetEssence", essenceValue);
                break;

            default:
                List<InventoryEntry> items = InventoryManager.Ref.GetItemListOfType(_ActiveSubscreen);

                foreach (InventoryEntry item in items)
                    Instantiate(_ItemMiniaturePrefab, _ItemGrid).SendMessage("SetItem", item);
            break;
        }
    }
}
