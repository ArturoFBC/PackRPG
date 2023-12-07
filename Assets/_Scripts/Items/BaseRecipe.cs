using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProductType
{
    BaseReference,
    SingleCharm,
    DoubleCharm,
    Bait
}

[System.Serializable]
public struct Product
{
    public ProductType type;
    public BaseItem item;
    public int tier;
}

[CreateAssetMenu(menuName = "Items/Recipe")]
[System.Serializable]
public class BaseRecipe : Droppable
{
    [SerializeField] private string name;

    [SerializeField] private List<Ingredient> ingredients;

    [SerializeField] private Product product;

    [SerializeField] private int price;

    public override void AddToInventory(int amount)
    {
        InventoryManager.Ref.UnlockRecipe(this);
    }

    public override string GetName()
    {
        return name;
    }

    public Product GetProduct()
    {
        return product;
    }

    public List<Ingredient> GetIngredients()
    {
        return ingredients;
    }

    public int GetPrice()
    {
        return price;
    }
}
