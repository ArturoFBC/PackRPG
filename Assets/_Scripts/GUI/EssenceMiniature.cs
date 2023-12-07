using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EssenceMiniature : MonoBehaviour
{
    [SerializeField] private Image _Icon;
    [SerializeField] private Text _Amount;
    [SerializeField] private SpeciesTooltipDisplayer _Tooltip;
    [SerializeField] private Image _SelectBorder;

    private Species mySpecies;

    public void OnEnable()
    {
        InventoryManager.SpecificEssencesChangedEvent += OnEssenceChanged;
    }

    public void OnDisable()
    {
        InventoryManager.SpecificEssencesChangedEvent -= OnEssenceChanged;
    }

    public void OnEssenceChanged()
    {
        _Amount.text = InventoryManager.Ref.GetEssenceInventory()[mySpecies].ToString();
    }

    public virtual void SetEssence(EssenceValue essence)
    {
        if (essence == null)
        {
            _Amount.enabled = false;
            _Icon.enabled = false;
            return;
        }

        _Amount.text = essence.amount.ToString();
        _Tooltip.SetObjectToDisplay(essence.species);
        _Icon.sprite = essence.species.miniature;
        mySpecies = essence.species;
    }

    public void OnSelect(BaseEventData eventData)
    {
        _SelectBorder.enabled = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _SelectBorder.enabled = false;
    }
}
