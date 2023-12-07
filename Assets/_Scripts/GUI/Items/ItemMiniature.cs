using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;

public class ItemMiniature : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public delegate void ItemClicked(Item miniature);
    static public event ItemClicked ItemClickedEvent;

    [SerializeField] private Image myIcon;
    [SerializeField] private Text amountLabel;
    [SerializeField] private ItemTooltipDisplayer myTooltip;
    [SerializeField] private Image mySelectBorder;

    private Transform myIconParent;

    protected Item _MyItem;

    public virtual void SetItem(InventoryEntry item)
    {
        if (item == null)
        {
            amountLabel.enabled = false;
            myIcon.enabled = false;
            return;
        }

        amountLabel.text = item.amount.ToString();
        myTooltip.SetObjectToDisplay(item.item);
        myIcon.sprite = item.item.GetIcon();

        _MyItem = item.item;
    }

    public Item GetItem()
    {
        return _MyItem;
    }

    public void OnSelect(BaseEventData eventData)
    {
        mySelectBorder.enabled = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        mySelectBorder.enabled = false;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        ItemClickedEvent?.Invoke(_MyItem);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        myIconParent = myIcon.rectTransform.parent;
        myIcon.rectTransform.SetParent(((RectTransform)transform).GetParentCanvas().transform);
        myIcon.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        myIcon.rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        myIcon.rectTransform.SetParent( myIconParent );
        myIcon.rectTransform.localPosition = Vector3.zero;
        myIcon.raycastTarget = true;
    }
}
