using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeList : MonoBehaviour
{
    [Header("Recipe")]
    [SerializeField] private GameObject recipeButtonPrefab;
    private List<RecipeButton> recipeButtons = new List<RecipeButton>();
    [SerializeField] private Transform recipeButtonsContainer;

    [SerializeField] private ToggleGroup myToggleGroup;

    public delegate void SelectedRecipeChanged(BaseRecipe newSelectedRecipe);
    public event SelectedRecipeChanged SelectedRecipeChangedEvent;

    private void Awake()
    {
        if (myToggleGroup == null)
            myToggleGroup = GetComponent<ToggleGroup>();
    }

    private void OnEnable()
    {
        foreach (RecipeButton recipeButton in recipeButtons)
            Destroy(recipeButton.gameObject);

        foreach (BaseRecipe recipe in InventoryManager.Ref.GetUnlockedRecipeList())
        {
            RecipeButton newRecipeButton = (Instantiate(recipeButtonPrefab, recipeButtonsContainer)).GetComponent<RecipeButton>();
            newRecipeButton.Set(recipe, myToggleGroup, SelectRecipe);
            recipeButtons.Add(newRecipeButton);
        }
    }

    public void SelectRecipe(BaseRecipe recipe)
    {
        SelectedRecipeChangedEvent?.Invoke(recipe);
    }
}
