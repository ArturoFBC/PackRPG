using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class UseItemButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField]
    private ConsumableItem myItem;

    [SerializeField] private Image myIcon;
    private Transform myIconParent;

    [SerializeField] private Text myItemAmount;

    [SerializeField] private KeyCode myKeyShortcut;

    
    private void Start()
    {
        //
        ItemSaveData saveData = new ItemSaveData()
        {
            itemType = (int)ItemType.CONSUMABLE,
            consumableType = (int)ConsumableType.BAIT,
            speciesIndex = 6
        };
        saveData.statsIncreased.Add((int)PrimaryStat.AGI, 3);
        BaitItem consumable = (BaitItem)ItemFactory.CreateItem(saveData);
        InventoryManager.Ref.AddItem(consumable);

        saveData = new ItemSaveData()
        {
            itemType = (int)ItemType.EQUIPMENT,
            equipmentType = (int)EquipmentType.CHARM,
            mainStatsIncreased = new List<int> { 1 },
            secondaryStatIncreased = 2,
            secondaryStatStrength = 0.4f
        };
        saveData.statsIncreased.Add((int)PrimaryStat.AGI, 3);
        StatModifierItem equipment = (StatModifierItem)ItemFactory.CreateItem(saveData);
        InventoryManager.Ref.AddItem(equipment);


        //
        PlayerInput.Ref.SetItemShortcut(myKeyShortcut, this);
    }

    public void SetItem(ConsumableItem myNewItem)
    {
        myItem = myNewItem;
        myIcon.enabled = true;
        myIcon.sprite = myNewItem.GetIcon();
        myItemAmount.enabled = true;
        myItemAmount.text = InventoryManager.Ref.GetItemAmount(myItem).ToString();
    }

    public TargetType GetTargetType()
    {
        return myItem.GetTargetType();
    }

    public void Reset()
    {
        myItem = null;
        myIcon.enabled = false;
        myItemAmount.enabled = false;
    }

    public void OnEnable()
    {
        Link();
    }

    public void OnDisable()
    {
        Unlink();
    }

    private void Unlink()
    {
        InventoryManager.InventoryChangedEvent -= OnInventoryChange;
    }

    private void Link()
    {
        InventoryManager.InventoryChangedEvent += OnInventoryChange;
    }

    private void OnInventoryChange()
    {
        int remainingAmount = InventoryManager.Ref.GetItemAmount(myItem);
        if (remainingAmount > 0)
            myItemAmount.text = remainingAmount.ToString();
        else
            Reset();
    }

    public void UseShortcut()
    {
        print("using item");
        PlayerInput.Ref.UseItem(this);
    }

    public ConsumableItem GetItem()
    {
        return myItem;
    }

    #region DRAG & DROP CALLBACKS
    public void OnBeginDrag(PointerEventData eventData)
    {
        myIconParent = myIcon.rectTransform.parent;
        myIcon.rectTransform.SetParent ( ((RectTransform)transform).GetParentCanvas().transform );
        myIcon.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        myIcon.rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        myIcon.rectTransform.SetParent ( myIconParent );
        myIcon.rectTransform.localPosition = Vector3.zero;
        myIcon.raycastTarget = true;

        // If dropped into inventory, clear the slot
        InventoryPanel inventoryPanel = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<InventoryPanel>();
        print(eventData.pointerCurrentRaycast);

        if (inventoryPanel != null)
            Reset();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // If a Consumable item is dropped on this, assign it
        ItemMiniature droppedItemMiniature = eventData.pointerDrag.GetComponent<ItemMiniature>();

        if (droppedItemMiniature == null)
            return;

        Item droppedItem = droppedItemMiniature.GetItem();

        if (droppedItem is ConsumableItem)
            SetItem((ConsumableItem)droppedItem);
    }
    #endregion
}
