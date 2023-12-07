using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraftingPanel : MonoBehaviour
{
    private enum IngredientState
    {
        None,
        SelectedSlot,
        SelectedItem,
    }

    #region RecipeData
    [Header("Recipe")]
    [SerializeField] private RecipeList recipeList;
    private BaseRecipe selectedRecipe;
    [SerializeField] private Text priceLabel;
    [SerializeField] private RecipeResultDisplay resultDisplay;
    #endregion

    #region IngredientData
    [Header("Ingredients")]
    [SerializeField] private GameObject IngredientSlotPrefab;
    private List<IngredientSlot> ingredientSlots = new List<IngredientSlot>();
    [SerializeField] private Transform ingredientSlotContainer;
    #endregion

    #region Dragging and selection
    [Header("Draggingandselection")]
    private Ingredient selectedIngredient;
    private IngredientSlot selectedSlot;
    private GameObject dragingItem;
    private IngredientState state = IngredientState.None;

    [SerializeField] private GameObject dragingItemPrefab;
    #endregion

    [SerializeField] private Button craftButton;

    public List<BaseItem> items = new List<BaseItem>();

    private void Awake()
    {
        if (recipeList == null)
            recipeList = GetComponentInChildren<RecipeList>();

        foreach (BaseItem bi in items)
            InventoryManager.Ref.AddItem(new ReferenceItem(bi));
    }

    private void OnEnable()
    {
        craftButton.enabled = false;

        recipeList.SelectedRecipeChangedEvent += OnRecipeChange;

        IngredientEventsSubscribe();
    }

    private void OnDisable()
    {
        recipeList.SelectedRecipeChangedEvent -= OnRecipeChange;

        IngredientEventsUnsubscribe();

        Deselect();
    }

    private void IngredientEventsUnsubscribe()
    {
        IngredientSlot.SlotClickedEvent -= IngredientSlotClick;
        IngredientSlot.SlotRightClickedEvent -= IngredientSlotRightClick;
        ScrapableDisplay.ScrapableClickedEvent -= IngredientClick;
        
    }

    private void IngredientEventsSubscribe()
    {
        IngredientSlot.SlotClickedEvent += IngredientSlotClick;
        IngredientSlot.SlotRightClickedEvent += IngredientSlotRightClick;
        ScrapableDisplay.ScrapableClickedEvent += IngredientClick;
    }

    #region Recipe
    private void OnRecipeChange( BaseRecipe recipe )
    {
        selectedRecipe = recipe;

        foreach ( Transform child in ingredientSlotContainer )
            Destroy(child.gameObject);

        ingredientSlots.Clear();
        foreach ( Ingredient ingredient in recipe.GetIngredients() )
        {
            IngredientSlot newIngredientSlot = (Instantiate(IngredientSlotPrefab, ingredientSlotContainer)).GetComponent<IngredientSlot>();
            newIngredientSlot.SetTemplate(ingredient);
            ingredientSlots.Add(newIngredientSlot);
        }

        priceLabel.text = recipe.GetPrice().ToString();
        priceLabel.color = (InventoryManager.Ref.GetEssence() >= recipe.GetPrice()) ? Color.white : Color.red;

        resultDisplay.Set(recipe.GetProduct());

        Deselect();

        craftButton.enabled = CheckIngredients();
    }
    #endregion

    #region ingredients
    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) &&
             (state != IngredientState.None))
        {
            if ((EventSystem.current.currentSelectedGameObject == null) ||
                 (state == IngredientState.SelectedItem && EventSystem.current.currentSelectedGameObject.GetComponent<IngredientSlot>() == null) ||
                 (state == IngredientState.SelectedSlot && (EventSystem.current.currentSelectedGameObject.GetComponent<ScrapableDisplay>() == null && EventSystem.current.currentSelectedGameObject.GetComponent<InventoryPanel>() == null)))
            {
                Deselect();
            }
        }
    }

    public void IngredientSlotClick(IngredientSlot slotDisplay)
    {
        if (state == IngredientState.SelectedItem)
        {
            slotDisplay.TrySetAssigned(selectedIngredient);
            craftButton.enabled = CheckIngredients();
            Deselect();
        }
        else
        {
            SelectSlot(slotDisplay);
        }
    }

    public void IngredientSlotRightClick(IngredientSlot slotDisplay)
    {
        selectedSlot.Unassign();

        Deselect();
    }

    public void IngredientClick(IScrapable ingredientAsScrappable)
    {
        Ingredient ingredient = new Ingredient(ingredientAsScrappable);

        if (state == IngredientState.SelectedSlot)
        {
            selectedSlot.TrySetAssigned(ingredient);
            craftButton.enabled = CheckIngredients();
            Deselect();
        }
        else
        {
            SelectIngredient(ingredient);
        }
    }

    public void EmptyInventorySpaceClick()
    {
        if (state == IngredientState.SelectedSlot)
        {
            selectedSlot.Unassign();

            Deselect();
        }
    }

    private void SelectIngredient(Ingredient ingredient)
    {
        Deselect();
        selectedIngredient = ingredient;

        dragingItem = CreateDragGraphic(ingredient);
        state = IngredientState.SelectedItem;
    }

    private void SelectSlot(IngredientSlot slotDisplay)
    {
        Deselect();
        selectedSlot = slotDisplay;

        Ingredient item = slotDisplay.GetIngredientAssigned();
        if (item != null)
            dragingItem = CreateDragGraphic(item);

        state = IngredientState.SelectedSlot;
    }

    private void Deselect()
    {
        selectedIngredient = null;

        selectedSlot = null;

        Destroy(dragingItem);

        state = IngredientState.None;
    }

    private GameObject CreateDragGraphic(Ingredient ingredient)
    {
        DraggedItem draggedItem = Instantiate(dragingItemPrefab, transform).GetComponent<DraggedItem>();
        draggedItem.SetImage(ingredient.GetSprite());
        return draggedItem.gameObject;
    }
    #endregion

    #region craft button
    private bool CheckIngredients()
    {
        List<Ingredient> ingredients = new List<Ingredient>();
        foreach (IngredientSlot slot in ingredientSlots)
        {
            if (slot.IsAssigned())
                ingredients.Add(slot.GetIngredientAssigned());
            else
                return false;
        }

        if (selectedRecipe.GetPrice() > InventoryManager.Ref.GetEssence())
            return false;

        return Crafting.CheckRecipeIngredients(ingredients);
    }

    public void Craft()
    {
        List<Ingredient> selectedIngredients = GetSelectedIngredients();
        if (selectedIngredients != null)
        {
            Item newItem = Crafting.CraftRecipe(selectedRecipe, selectedIngredients);
        }

        craftButton.enabled = CheckIngredients();
    }
    #endregion

    private List<Ingredient> GetSelectedIngredients()
    {
        List<Ingredient> ingredients = new List<Ingredient>();
        foreach (IngredientSlot slot in ingredientSlots)
        {
            if (slot.IsAssigned())
                ingredients.Add(slot.GetIngredientAssigned());
            else
                return null;
        }
        return ingredients;
    }
}