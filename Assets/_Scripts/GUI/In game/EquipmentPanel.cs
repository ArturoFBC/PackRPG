using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentPanel : MonoBehaviour
{
    private enum EquipmentState
    {
        None,
        SelectedSlot,
        SelectedItem,
    }

    #region SCENE_REFERENCES
    [SerializeField] private EquipmentDisplay equipmentDisplay;
    [SerializeField] private InventoryPanel inventoryPanel;
    #endregion

    #region PREFAV_REFERENCES
    [SerializeField] private GameObject dragingItemPrefab;
    #endregion

    private GameObject dragingItem;

    private Item selectedItem;
    private EquipmentSlotDisplay selectedSlot;

    private EquipmentState state = EquipmentState.None;

    private void Awake()
    {
        if (inventoryPanel == null)
            inventoryPanel = GetComponentInChildren<InventoryPanel>();

        if (equipmentDisplay == null)
            equipmentDisplay = GetComponentInChildren<EquipmentDisplay>();
    }

    private void OnEnable()
    {
        inventoryPanel.ShowEquipment(true);

        EquipmentSlotDisplay.SlotClickedEvent += EquipmentSlotClick;
        EquipmentSlotDisplay.SlotRightClickedEvent += EquipmentSlotRightClick;
        ItemMiniature.ItemClickedEvent += ItemClick;
    }

    private void OnDisable()
    {
        Deselect();

        EquipmentSlotDisplay.SlotClickedEvent -= EquipmentSlotClick;
        EquipmentSlotDisplay.SlotRightClickedEvent -= EquipmentSlotRightClick;
        ItemMiniature.ItemClickedEvent -= ItemClick;
    }

    private void Update()
    {
        if ( ( Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) ) &&
             ( state != EquipmentState.None ) )
        {
            if ( ( EventSystem.current.currentSelectedGameObject == null ) || 
                 ( state == EquipmentState.SelectedItem && EventSystem.current.currentSelectedGameObject.GetComponent<EquipmentSlotDisplay>() == null) ||
                 ( state == EquipmentState.SelectedSlot && (EventSystem.current.currentSelectedGameObject.GetComponent<EquipmentItemMiniture>() == null && EventSystem.current.currentSelectedGameObject.GetComponent<InventoryPanel>() == null) ) )
            {
                Deselect();
            }
        }
    }

    public void EquipmentSlotClick( EquipmentSlotDisplay slotDisplay )
    {
        if ( state == EquipmentState.SelectedItem )
        {
            Specimen selectedSpecimen = GetComponent<CreatureDisplay>().GetSpecimen();

            if ((EquipmentType)selectedItem.GetSaveData().equipmentType == slotDisplay.GetSlot().type)
                selectedSpecimen.EquipItem(selectedItem, slotDisplay.GetSlot());

            Deselect();
        }
        else
        {
            SelectSlot(slotDisplay);
        }
    }

    public void EquipmentSlotRightClick(EquipmentSlotDisplay slotDisplay)
    {
        Specimen selectedSpecimen = GetComponent<CreatureDisplay>().GetSpecimen();
        selectedSpecimen.UnequipItem(slotDisplay.GetSlot());

        Deselect();
    }

    public void ItemClick(Item item)
    {
        if (state == EquipmentState.SelectedSlot)
        {
            Specimen selectedSpecimen = GetComponent<CreatureDisplay>().GetSpecimen();

            if ( (EquipmentType)item.GetSaveData().equipmentType == selectedSlot.GetSlot().type )
                selectedSpecimen.EquipItem(item, selectedSlot.GetSlot());

            Deselect();
        }
        else
        {
            SelectItem(item);
        }
    }

    public void EmptyInventorySpaceClick()
    {
        if (state == EquipmentState.SelectedSlot)
        {
            Specimen selectedSpecimen = GetComponent<CreatureDisplay>().GetSpecimen();
            selectedSpecimen.UnequipItem(selectedSlot.GetSlot());

            Deselect();
        }
    }

    private void SelectItem(Item item )
    {
        Deselect();
        selectedItem = item;

        dragingItem = CreateDragGraphic(item);
        state = EquipmentState.SelectedItem;
    }

    private void SelectSlot ( EquipmentSlotDisplay slotDisplay )
    {
        Deselect();
        selectedSlot = slotDisplay;

        Item item = slotDisplay.GetSlot().item;
        if ( item != null )
            dragingItem = CreateDragGraphic( item);

        state = EquipmentState.SelectedSlot;
    }

    private void Deselect()
    {
        selectedItem = null;

        selectedSlot = null;

        Destroy(dragingItem);

        state = EquipmentState.None;
    }

    private GameObject CreateDragGraphic(Item item )
    {
        DraggedItem draggedItem = Instantiate(dragingItemPrefab, transform).GetComponent<DraggedItem>();
        draggedItem.SetImage(item.GetIcon());
        return draggedItem.gameObject;
    }
}
