using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    Item,
    Essence
}

[System.Serializable]
public class Ingredient
{
    public bool specific;
    public IngredientType type;

    #region Item ingredient
    public MaterialType category;
    public BaseMaterialItem item;
    public int tier;
    #endregion

    #region Essence ingredient
    public Species essence;
    #endregion

    [SerializeField] public int amount;

    public static bool Compare(Ingredient A, Ingredient B)
    {
        if (A.type != B.type)
            return false;

        if (A.type == IngredientType.Item)
        {
            return (A.item == B.item);
        }
        else if (A.type == IngredientType.Essence)
        {
            return (A.essence == B.essence);
        }

        return (A.tier == B.tier);
    }

    public bool Match(Ingredient ingredient)
    {
        if (type != ingredient.type)
            return false;

        if (type == IngredientType.Essence)
            return Match(ingredient.essence);
        else
            return Match(ingredient.item);
    }

    public bool Match(Species species)
    {
        if (type != IngredientType.Essence)
            return false;

        if (!specific)
            return true;

        return species == essence;
    }

    public bool Match(BaseMaterialItem itemCompare)
    {
        if (type != IngredientType.Item)
            return false;

        if (specific)
            return item == itemCompare;

        return (category == itemCompare.materialType && tier == itemCompare.tier);
    }

    public void Consume()
    {
        switch (type)
        {
            case IngredientType.Item:
                InventoryManager.Ref.RemoveItem(new ReferenceItem(item), amount);
                break;
            case IngredientType.Essence:
                InventoryManager.Ref.SpendEssence(essence, amount);
                break;
        }
    }

    public Ingredient(IScrapable scrapable)
    {
        specific = true;
        type = (scrapable is Item)? IngredientType.Item : IngredientType.Essence;
        if (type == IngredientType.Essence)
            essence = ((Essence)scrapable).GetSpecies();
        else
        {
            if (scrapable is ReferenceItem)
            {
                item = (BaseMaterialItem)((ReferenceItem)scrapable).baseItem;
                tier = item.tier;
            }
            else
                Debug.LogError("Can not create ingredient from an scrappable that is not a ReferenceItem");
        }
    }

    public Sprite GetSprite()
    {
        if (specific)
        {
            switch (type)
            {
                case IngredientType.Item:
                    return item.icon;
                case IngredientType.Essence:
                    return essence.miniature;
            }
        }
        else
        {
            Texture2D returnTexture = null;

            switch (type)
            {
                case IngredientType.Item:
                    switch (category)
                    {
                        case MaterialType.Crystal:
                            returnTexture = Resources.Load<Texture2D>("IngredientTypeIcons/crystal");
                        break;
                        case MaterialType.Gas:
                            returnTexture = Resources.Load<Texture2D>("IngredientTypeIcons/gas");
                            break;
                        case MaterialType.Glue:
                            returnTexture = Resources.Load<Texture2D>("IngredientTypeIcons/glue");
                            break;
                        case MaterialType.Metal:
                            returnTexture = Resources.Load<Texture2D>("IngredientTypeIcons/metal");
                            break;
                        case MaterialType.Wood:
                            returnTexture = Resources.Load<Texture2D>("IngredientTypeIcons/metal");
                            break;
                        case MaterialType.Stat_core:
                            returnTexture = Resources.Load<Texture2D>("IngredientTypeIcons/stat-core");
                            break;
                    }
                break;
                case IngredientType.Essence:
                    returnTexture = Resources.Load<Texture2D>("IngredientTypeIcons/dna");
                break;
            }

            if (returnTexture != null)
            {
                return Sprite.Create(returnTexture, new Rect(0, 0, returnTexture.width, returnTexture.height), new Vector2(0.5f, 0.5f));
            }
    }
        return null;
    }
}