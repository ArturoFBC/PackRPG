using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class InventoryEntry
{
    public Item item;
    public int amount;
}

public class InventoryManager : Singleton<InventoryManager>
{
    private int _GeneralEssence;
    public delegate void EssenceAmountChanged(int amount);
    public event EssenceAmountChanged EssenceAmountChangedEvent;

    public EssenceInventory _SpecificEssences = new EssenceInventory();

    public delegate void SpecificEssencesChanged();
    static public event SpecificEssencesChanged SpecificEssencesChangedEvent;

    public List<InventoryEntry> _Items = new List<InventoryEntry>();
    public delegate void InventoryChanged();
    static public event InventoryChanged InventoryChangedEvent;

    public List<BaseRecipe> _Recipes = new List<BaseRecipe>();
    public delegate void RecipeUnlocked(BaseRecipe unlockedRecipe);
    static public event RecipeUnlocked RecipeUnlockedEvent;

    [SerializeField] private InventoryEditor inventoryEditor;


    protected override void InheritedAwake()
    {
        _GeneralEssence   = inventoryEditor._GeneralEssence;
        _SpecificEssences = inventoryEditor._SpecificEssences;
        _Items = new List<InventoryEntry>(inventoryEditor._Items);
    }

    public EssenceInventory GetEssenceInventory()
    {
        return _SpecificEssences;
    }

    #region MANAGE_ITEMS
    public void AddItem(Item item, int amount = 1 )
    {
        bool found = false;
        for ( int i = 0; i < _Items.Count; i++ )
        {
            if ( _Items[i].item.AreEqual(item) )
            {
                _Items[i].amount += amount;
                found = true;
                break;
            }
        }

        if (found == false)
        {
            InventoryEntry newInventoryItem = new InventoryEntry();
            newInventoryItem.item = item;
            newInventoryItem.amount = amount;
            _Items.Add(newInventoryItem);
        }

        InventoryChangedEvent?.Invoke();
    }

    public int RemoveItem(Item item, int amount = 1)
    {
        int remaining = -1;

        int index = GetItemIndex(item);
        if (index > -1)
        {
            if (_Items[index].amount >= amount)
            {
                _Items[index].amount -= amount;
                if (_Items[index].amount == 0)
                {
                    _Items.RemoveAt(index);
                    remaining = 0;
                }
                else
                {
                    remaining = _Items[index].amount;
                }
            }

            InventoryChangedEvent?.Invoke();
        }

        return remaining;
    }

    public bool CheckItemStock(Item item, int amount = 1)
    {
        int index = GetItemIndex(item);
        if (index > -1)
            return (index > -1 || _Items[index].amount >= amount);
        else
            return amount == 0;
    }

    public int GetItemAmount(Item item)
    {
        int index = GetItemIndex(item);
        if (index > -1)
            return _Items[index].amount;
        else
            return 0;
    }

    private int GetItemIndex(Item item)
    {
        int index = -1;
        for (int i = 0; i < _Items.Count; i++)
        {
            if (_Items[i].item.AreEqual(item))
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public List<InventoryEntry> GetItemListOfType( ItemType requestedType )
    {
        List<InventoryEntry> returnItems = new List<InventoryEntry>();

        for (int i = 0; i < _Items.Count; i++)
        {
            if (requestedType == _Items[i].item.GetItemType())
                returnItems.Add(_Items[i]);
        }

        return returnItems;
    }
    #endregion

    #region MANAGE_GENERAL_ESSENCE
    public void AddEssence( int amount )
    {
        _GeneralEssence += amount;
        EssenceAmountChangedEvent?.Invoke(_GeneralEssence);
    }

    public void SpendEssence( int amount )
    {
        _GeneralEssence -= amount;
        EssenceAmountChangedEvent?.Invoke(_GeneralEssence);
    }

    public int GetEssence()
    {
        return _GeneralEssence;
    }
    #endregion

    #region MANAGE_SPECIFIC_ESSENCES
    public void AddEssence(Species species, int amount)
    {
        if (_SpecificEssences.ContainsKey(species))
            _SpecificEssences[species] += amount;
        else
            _SpecificEssences.Add(species, amount);

        SpecificEssencesChangedEvent?.Invoke();
    }

    public void SpendEssence(Species species, int amount)
    {
        if (_SpecificEssences.ContainsKey(species) && _SpecificEssences[species] >= amount)
            _SpecificEssences[species] -= amount;
        else
            Debug.LogError( "Trying to remove " + amount.ToString() + " essence of species " + species.name + " but there is not enough available" );

        SpecificEssencesChangedEvent?.Invoke();
    }

    public int GetEssenceAmount(Species species)
    {
        int essenceAmount = 0;
        if (_SpecificEssences.ContainsKey(species))
            essenceAmount = _SpecificEssences[species];

        return essenceAmount;
    }
    #endregion

    public int GetAmount(IScrapable scrapable)
    {
        if (scrapable is Item)
            return GetItemAmount((Item)scrapable);
        else if (scrapable is Essence)
            return GetEssenceAmount(((Essence)scrapable).GetSpecies());
        else
            return 0;
    }

    #region MANAGE_RECIPES
    public void UnlockRecipe( BaseRecipe unlockedRecipe )
    {
        if ( _Recipes.Contains(unlockedRecipe) == false )
        {
            _Recipes.Add(unlockedRecipe);
            RecipeUnlockedEvent?.Invoke(unlockedRecipe);
        }
    }

    public bool IsRecipeUnlocked( BaseRecipe queryRecipe )
    {
        return _Recipes.Contains(queryRecipe);
    }

    public List<BaseRecipe> GetUnlockedRecipeList()
    {
        List<BaseRecipe> returnRecipeList = new List<BaseRecipe>(_Recipes);
        return returnRecipeList;
    }
    #endregion

    public static void Load( InventoryManagerData data )
    {
        Reset();

        Ref._GeneralEssence = data._GeneralEssence;

        foreach (KeyValuePair<int, int> pair in data._SpecificEssences)
            Ref._SpecificEssences.Add(ScriptableReferencesHolder.GetSpeciesReference(pair.Key), pair.Value);

        foreach (KeyValuePair<ItemSaveData, int> pair in data._Items)
            Ref._Items.Add( new InventoryEntry { item = ItemFactory.CreateItem(pair.Key), amount = pair.Value });

        if (data._UnlockedRecipes != null)
            foreach (int recipeIndex in data._UnlockedRecipes)
                Ref._Recipes.Add(ScriptableReferencesHolder.GetRecipeReference(recipeIndex));

        InventoryChangedEvent?.Invoke();
    }

    public static void Reset()
    {
        Ref._GeneralEssence = 30;
        Ref._SpecificEssences = new EssenceInventory();
        Ref._Items = new List<InventoryEntry>();
        Ref._Recipes = new List<BaseRecipe>();
    }
}
