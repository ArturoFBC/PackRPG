using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Crafting
{
    
    public static bool CheckRecipeIngredients( BaseRecipe recipe )
    {
        return CheckRecipeIngredients(recipe.GetIngredients());
    }

    public static bool CheckRecipeIngredients( List<Ingredient> ingredients )
    {
        bool allIngredientsPresent = true;
        foreach (Ingredient ingredient in ingredients)
        {
            if ( CheckIngredient(ingredient) == false )
            {
                allIngredientsPresent = false;
                break;
            }
        }
        return allIngredientsPresent;
    }

    private static bool CheckIngredient(Ingredient ingredient)
    {
        switch (ingredient.type)
        {
            case IngredientType.Item:
                return InventoryManager.Ref.CheckItemStock(new ReferenceItem(ingredient.item), ingredient.amount);
            case IngredientType.Essence:
                return InventoryManager.Ref.GetEssenceInventory()[ingredient.essence] >= ingredient.amount;
        }

        return false;
    }

    public static Item CraftRecipe( BaseRecipe recipe, List<Ingredient> ingredients )
    {
        Product product = recipe.GetProduct();

        Item result = CreateItem(product, ingredients, recipe.GetPrice());

        RemoveIngredients(ingredients);

        return result;
    }

    private static Item CreateItem( Product product, List<Ingredient> ingredients, int price)
    {
        Item returnItem = null;

        if (product.type == ProductType.BaseReference)
        {
            returnItem = new ReferenceItem(product.item);
        }
        else if (product.type == ProductType.SingleCharm || product.type == ProductType.DoubleCharm)
        {
            List<PrimaryStat> stats = new List<PrimaryStat>();
            foreach (Ingredient ingredient in ingredients)
            {
                if (ingredient.item.materialType == MaterialType.Stat_core)
                    if ( stats.Contains(ingredient.item.stat) == false )
                        stats.Add(ingredient.item.stat);
            }

            if (stats != null && stats.Count > 0)
                returnItem = new StatModifierItem(product.tier, stats);

        }
        else if (product.type == ProductType.Bait)
        {
            Dictionary<PrimaryStat, float> stats = new Dictionary<PrimaryStat, float>();
            Species targetSpecies = null;
            foreach (Ingredient ingredient in ingredients)
            {
                switch (ingredient.type)
                {
                    case IngredientType.Item:
                        if (ingredient.item.materialType == MaterialType.Stat_core)
                        {
                            if (stats.ContainsKey(ingredient.item.stat) == false)
                                stats.Add(ingredient.item.stat, 2 * (product.tier + 1));

                            Debug.Log(stats[ingredient.item.stat]);
                        }
                        break;
                    case IngredientType.Essence:
                        if (ingredient.essence != null)
                        {
                            targetSpecies = ingredient.essence;
                        }
                        break;
                }
            }

            if (targetSpecies == null)
                Debug.LogError("Tried to create a bait without an target species!");

            returnItem = new BaitItem(targetSpecies, stats);
        }

        InventoryManager.Ref.AddItem(returnItem);
        InventoryManager.Ref.SpendEssence(price);
        return returnItem;
    }

    private static void RemoveIngredients(List<Ingredient> ingredients)
    {
        foreach (Ingredient ingredient in ingredients)
        {
            ingredient.Consume();
        }
    }
}
