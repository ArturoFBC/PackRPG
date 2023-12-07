using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IHasIcon
{
    Sprite GetIcon();
}

public class SelectOrDragIntoSlot<ContentType,SlotType>
    where SlotType : Slot<ContentType>
    where ContentType : IComparable, IHasIcon
{
    private enum AssignationState
    {
        None,
        SelectedSlot,
        SelectedItem,
    }

    public delegate void ContentSlotted(SlotType slot, ContentType content);
    public event ContentSlotted ContentSlottedEvent;
    public event ContentSlotted ContentRemovedFromSlotEvent;

    private ContentType selectedContent;
    private SlotType selectedSlot;
    private AssignationState state;
    private GameObject draggingItem;

    /*
    private void IngredientEventsUnsubscribe()
    {
        IngredientSlot.SlotClickedEvent -= SlotClick;
        IngredientSlot.SlotRightClickedEvent -= SlotRightClick;
        ScrapableDisplay.ScrapableClickedEvent -= ContentClick;

    }

    private void IngredientEventsSubscribe()
    {
        IngredientSlot.SlotClickedEvent -= SlotClick;
        IngredientSlot.SlotRightClickedEvent -= SlotRightClick;
        ScrapableDisplay.ScrapableClickedEvent -= ContentClick;
    }
    */

    /// <summary>
    /// Try to put content into slot
    /// </summary>
    private void SetSlotContent(SlotType slot, ContentType content)
    {
        if ( slot.ValidateContent(content) )
            slot.SlotContent(content);
    }

    public void SlotClick(SlotType slotClicked)
    {
        if (state == AssignationState.SelectedItem)
        {
            SetSlotContent(slotClicked, selectedContent);
            Deselect();
        }
        else
        {
            SelectSlot(slotClicked);
        }
    }

    public void ContentClick(ContentType contentClicked)
    {
        if (state == AssignationState.SelectedSlot)
        {
            SetSlotContent(selectedSlot,contentClicked);
            Deselect();
        }
        else
        {
            SelectContent(contentClicked);
        }
    }

    public void SlotRightClick(SlotType slotDisplay)
    {
        slotDisplay.Unslot();

        Deselect();
    }

    private void SelectContent(ContentType newSelectedContent)
    {
        Deselect();
        selectedContent = newSelectedContent;

        draggingItem = CreateDragGraphic(newSelectedContent);
        state = AssignationState.SelectedItem;
    }

    private void SelectSlot(SlotType newSelectedSlot)
    {
        Deselect();
        selectedSlot = newSelectedSlot;

        ContentType slotContent = selectedSlot.GetContent();
        if (slotContent.Equals(default) == false)
            draggingItem = CreateDragGraphic(slotContent);

        state = AssignationState.SelectedSlot;
    }

    private void Deselect()
    {
        selectedContent = default;

        selectedSlot = default;

        GameObject.Destroy( draggingItem );

        state = AssignationState.None;
    }

    private GameObject CreateDragGraphic(ContentType item)
    {
        GameObject draggedItemGO = new GameObject("DraggedObject");

        draggedItemGO.AddComponent<RectTransform>();
        draggedItemGO.AddComponent<Image>();
        DraggedItem draggedItem = draggedItemGO.AddComponent<DraggedItem>();
        draggedItem.SetImage(item.GetIcon());

        return draggingItem.gameObject;
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) &&
             (state != AssignationState.None))
        {
            if ((EventSystem.current.currentSelectedGameObject == null) ||
                 (state == AssignationState.SelectedItem && EventSystem.current.currentSelectedGameObject.GetComponent<SlotType>() == null) ||
                 (state == AssignationState.SelectedSlot && (EventSystem.current.currentSelectedGameObject.GetComponent<ContentType>() == null && EventSystem.current.currentSelectedGameObject.GetComponent<InventoryPanel>() == null)))
            {
                Deselect();
            }
        }
    }
}
