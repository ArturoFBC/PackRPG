using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlotDisplay : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    public delegate void SlotClicked(EquipmentSlotDisplay slot);
    static public event SlotClicked SlotClickedEvent;
    static public event SlotClicked SlotRightClickedEvent;

    [SerializeField] private ItemTooltipDisplayer tooltip;
    [SerializeField] private Text itemName;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image background;
    [SerializeField] private Image selectBorder;

    [SerializeField] private EquipmentSlot _MySlot;
    public EquipmentSlot mySlot
    {
        set
        {
            if (_MySlot != null)
                _MySlot.ItemChangedEvent -= OnItemChanged;

            _MySlot = value;

            if (_MySlot != null)
                _MySlot.ItemChangedEvent += OnItemChanged;
        }
        get
        {
            return _MySlot;
        }
    }

    [SerializeField] private Sprite[] backgroundImages;

    public void Awake()
    {
        if (tooltip == null)
            tooltip = GetComponent<ItemTooltipDisplayer>();

        if (tooltip == null)
            Debug.LogError("ItemTooltipDisplayer not set nor found in " + gameObject.name);
    }

    private void OnDisable()
    {
        if (mySlot != null)
            mySlot.ItemChangedEvent -= OnItemChanged;
    }

    public void OnSelect(BaseEventData eventData)
    {
        selectBorder.enabled = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selectBorder.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            SlotClickedEvent?.Invoke( this );
        else if (eventData.button == PointerEventData.InputButton.Right)
            SlotRightClickedEvent?.Invoke( this );
    }

    public void OnItemChanged( Item newItem, Item oldItem )
    {
        UpdateSlotContent();
    }

    public void Display( EquipmentSlot slot )
    {
        mySlot = slot;

        if (background != null && slot.type== EquipmentType.CHARM)
        {
            Vector2 newDelta = background.rectTransform.sizeDelta;
            newDelta.y = newDelta.x * 0.8f;
            background.rectTransform.sizeDelta = newDelta;
        }

        UpdateSlotContent();
    }

    private void UpdateSlotContent()
    {
        if (mySlot.item != null)
        {
            if (tooltip == null)
                tooltip = GetComponent<ItemTooltipDisplayer>();

            tooltip.SetObjectToDisplay(mySlot.item);
            tooltip.enabled = true;

            if (itemName != null)
            {
                itemName.text = mySlot.item.GetName();
                itemName.enabled = true;
            }
            if (itemIcon != null)
            {
                itemIcon.sprite = mySlot.item.GetIcon();
                itemIcon.enabled = true;
            }
        }
        else
        {
            GoBlank();
        }
    }

    private void GoBlank()
    {
        if (tooltip != null)
            tooltip.enabled = false;

        if (itemName != null)
            itemName.enabled = false;
        if (itemIcon != null)
            itemIcon.enabled = false;
    }

    public EquipmentSlot GetSlot()
    {
        return mySlot;
    }
}
