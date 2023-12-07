using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrapingPanel : MonoBehaviour
{
    [SerializeField] private Slider amountSlider;
    [SerializeField] private Button scrapButton;
    [SerializeField] private Image selectedScrapableIcon;
    [SerializeField] private Text scrapResultLabel;
    [SerializeField] private InventoryPanel myInventoryPanel;

    private IScrapable selectedScrapable;

    private void OnEnable()
    {
        SetSelected(null);
        ScrapableDisplay.ScrapableClickedEvent += SetSelected;
    }

    private void OnDisable()
    {
        ScrapableDisplay.ScrapableClickedEvent -= SetSelected;
    }


    public void Scrap()
    {
        if ( selectedScrapable != null && amountSlider.value > 0 )
        {
            selectedScrapable.Scrap( (int)amountSlider.value );
            SetSelected(null);
        }
    }

    public void SetSelected( IScrapable scrapable )
    {
        selectedScrapable = scrapable;
        amountSlider.interactable = (scrapable != null);
        scrapButton.interactable = (scrapable != null);
        selectedScrapableIcon.enabled = (scrapable != null);

        if (scrapable != null)
        {
            int scrapableInventoryAmount = InventoryManager.Ref.GetAmount( scrapable );
            if (scrapableInventoryAmount > 0)
            {
                selectedScrapable = scrapable;
                amountSlider.maxValue = scrapableInventoryAmount;
                amountSlider.value = 1;
                selectedScrapableIcon.sprite = selectedScrapable.GetIcon();
            }
        }
    }

    public void SetAmount( float amount )
    {
        scrapResultLabel.text = ((int)amount * selectedScrapable.GetCurrencyValue()).ToString();
    }
}
