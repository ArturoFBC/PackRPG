using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[System.Serializable]
public class ItemSaveData
{
    public int itemType;
    public int equipmentType;
    public int consumableType;

    public int baseItemIndex;

    public int tier;

    #region StatIncrease
    public List<int> mainStatsIncreased = new List<int>();
    public int secondaryStatIncreased;
    public float secondaryStatStrength;
    #endregion

    #region BaitItem
    public int speciesIndex;
    public Dictionary<int, float> statsIncreased = new Dictionary<int, float>();
    #endregion

    #region PillItem
    public int statIncrease;
    #endregion
}

public static class ItemFactory
{
    public static Item CreateItem( ItemSaveData saveData )
    {
        if ((ItemType)saveData.itemType == ItemType.EQUIPMENT &&
             (EquipmentType)saveData.equipmentType == EquipmentType.CHARM)
            return new StatModifierItem(saveData);
        else if ((ItemType)saveData.itemType == ItemType.CONSUMABLE &&
            (ConsumableType)saveData.consumableType == ConsumableType.BAIT)
            return new BaitItem(saveData);
        else
            return new ReferenceItem(saveData);
    }

    public static Item CreateItem(BaseItem baseItem)
    {
        return new ReferenceItem(baseItem);
    }
}

[System.Serializable]
public abstract class Item: IScrapable
{
    public int tier;
    public ItemType type;
    public EquipmentType equipmentType;

    protected Item()
    {
    }

    protected Item(ItemSaveData saveData)
    {
        tier = saveData.tier;
        type = (ItemType)saveData.itemType;
        equipmentType = (EquipmentType)saveData.equipmentType;
    }

    public virtual ItemSaveData GetSaveData()
    {
        ItemSaveData returnSaveData = new ItemSaveData()
        {
            itemType = (int)type,
            equipmentType = (int)equipmentType,
            tier = tier
        };

        return returnSaveData;
    }

    public abstract bool AreEqual(Item otherItem);

    public abstract string GetDescription();

    public abstract Sprite GetIcon();

    public abstract ItemType GetItemType();

    public abstract string GetName();

    public abstract int GetCurrencyValue();

    public void Scrap(int amount)
    {
        InventoryManager.Ref.RemoveItem(this, amount);
        InventoryManager.Ref.AddEssence(amount * GetCurrencyValue());
    }
}
