using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    public delegate void SlotClicked(IngredientSlot slot);
    static public event SlotClicked SlotClickedEvent;
    static public event SlotClicked SlotRightClickedEvent;

    [SerializeField] private Ingredient myIngredientTemplate;
    private Ingredient myIngredientAssigned;

    #region Display
    [SerializeField] private Image myImage;
    [SerializeField] private Text amountRequired;
    [SerializeField] private Text amountInInventory;
    [SerializeField] private Text nameDisplay;
    [SerializeField] private Text tierDisplay;
    #endregion

    [SerializeField] private Image selectBorder;

    public void SetTemplate( Ingredient ingredient )
    {
        myIngredientTemplate = ingredient;
        if (myIngredientTemplate.specific == true)
            myIngredientAssigned = myIngredientTemplate;

        DisplayIngredient(ingredient);
    }

    public bool TrySetAssigned( Ingredient assignedCandidate )
    {
        if ( myIngredientTemplate == null || myIngredientTemplate.specific == true )
            return false;

        if (myIngredientTemplate.Match(assignedCandidate))
        {
            myIngredientAssigned = assignedCandidate;
            myIngredientAssigned.amount = myIngredientTemplate.amount;
            DisplayIngredient(myIngredientAssigned);
            return true;
        }

        return false;
    }

    public Ingredient GetIngredientAssigned()
    {
        return myIngredientAssigned;
    }

    public void Unassign()
    {
        myIngredientAssigned = null;
        DisplayIngredient(myIngredientTemplate);
    }

    public bool IsAssigned()
    {
        if (myIngredientAssigned != null)
            return true;

        return false;
    }

    #region CALLBACKS
    public void OnEnable()
    {
        InventoryManager.SpecificEssencesChangedEvent += OnEssenceAmountchange;
        InventoryManager.InventoryChangedEvent += OnItemAmountChange;
    }

    public void OnDisable()
    {
        InventoryManager.SpecificEssencesChangedEvent -= OnEssenceAmountchange;
        InventoryManager.InventoryChangedEvent -= OnItemAmountChange;
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
            SlotClickedEvent?.Invoke(this);
        else if (eventData.button == PointerEventData.InputButton.Right)
            SlotRightClickedEvent?.Invoke(this);
    }
    #endregion

    private void DisplayIngredient(Ingredient ingredient)
    {
        myImage.sprite = ingredient.GetSprite();

        amountRequired.text = myIngredientTemplate.amount.ToString();

        if (ingredient.specific)
        {
            if (ingredient.type == IngredientType.Essence)
                amountInInventory.text = InventoryManager.Ref._SpecificEssences[ingredient.essence].ToString();
            else if (ingredient.type == IngredientType.Item)
                amountInInventory.text = InventoryManager.Ref.GetItemAmount(new ReferenceItem(ingredient.item)).ToString();
        }
        else
            amountInInventory.text = "-";
    }

    private void OnItemAmountChange()
    {
        if (myIngredientAssigned != null && myIngredientAssigned.type == IngredientType.Item)
            amountInInventory.text = InventoryManager.Ref.GetItemAmount(new ReferenceItem(myIngredientAssigned.item)).ToString();
    }

    private void OnEssenceAmountchange()
    {
        if ( myIngredientAssigned != null && myIngredientAssigned.type == IngredientType.Essence)
        amountInInventory.text = InventoryManager.Ref._SpecificEssences[myIngredientAssigned.essence].ToString();
    }
}
