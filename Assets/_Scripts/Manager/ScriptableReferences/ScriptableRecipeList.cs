using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/RecipesList")]
public class ScriptableRecipeList : ScriptableObject
{
    public List<BaseRecipe> recipes;
}
