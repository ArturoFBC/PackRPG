using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrapableDisplay : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    public delegate void ScrapableClicked(IScrapable scrapable);
    static public event ScrapableClicked ScrapableClickedEvent;

    [SerializeField] private Image icon;
    [SerializeField] private Text amount;
 //   [SerializeField] private ItemTooltipDisplayer tooltip;
    [SerializeField] private Image selectBorder;

    private IScrapable myScrapable;

    public void SetEssence(EssenceValue scrapable)
    {
        if ( scrapable == null )
        {
            amount.enabled = false;
            icon.enabled = false;
            return;
        }

        amount.text = scrapable.amount.ToString();
        icon.sprite = scrapable.species.miniature;
        myScrapable = new Essence(scrapable.species);
    }

    public void SetItem(InventoryEntry scrapable)
    {
        if (scrapable == null || (scrapable.item is IScrapable) == false)
        {
            amount.enabled = false;
            icon.enabled = false;
            return;
        }

        amount.text = scrapable.amount.ToString();
        SetItem(scrapable.item);
    }

    public void SetItem(Item scrapable)
    {
        if (scrapable == null || (scrapable is IScrapable) == false)
        {
            amount.enabled = false;
            icon.enabled = false;
            return;
        }

        icon.sprite = scrapable.GetIcon();
        myScrapable = scrapable;
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
        ScrapableClickedEvent?.Invoke(myScrapable);
    }
}
