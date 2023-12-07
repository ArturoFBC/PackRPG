using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeResultDisplay : MonoBehaviour
{
    [SerializeField] private Image iconDisplay;
    [SerializeField] private ItemTooltipDisplayer itemTooltipDisplayer;

    private void Awake()
    {
        if (itemTooltipDisplayer == null)
        {
            itemTooltipDisplayer = GetComponent<ItemTooltipDisplayer>();
        }
    }

    public void Set(Product product)
    {
        switch (product.type)
        {
            case ProductType.BaseReference:
                iconDisplay.sprite = product.item.icon;
                itemTooltipDisplayer.SetObjectToDisplay( new ReferenceItem( product.item ) );
                itemTooltipDisplayer.enabled = true;
                break;
            default:
                iconDisplay.sprite = StatModifierItem.GetNeutralIcon();
                itemTooltipDisplayer.enabled = false;
                break;
        }
    }


}
