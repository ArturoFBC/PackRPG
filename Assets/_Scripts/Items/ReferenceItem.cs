using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReferenceItem : Item
{
    public BaseItem baseItem;

    public ReferenceItem(ItemSaveData saveData) : base(saveData)
    {
        baseItem = ScriptableReferencesHolder.GetItemReference( saveData.baseItemIndex );
    }

    public ReferenceItem(BaseItem baseItem)
    {
        type = baseItem.GetItemType();
        this.baseItem = baseItem;
    }

    public override ItemSaveData GetSaveData()
    {
        ItemSaveData saveData = base.GetSaveData();

        saveData.baseItemIndex = ScriptableReferencesHolder.GetItemIndex(baseItem);

        return saveData;
    }

    public override string GetDescription()
    {
        return baseItem?.description;
    }

    public override Sprite GetIcon()
    {
        return baseItem?.icon;
    }

    public override ItemType GetItemType()
    {
        return baseItem.GetItemType();
    }

    public override string GetName()
    {
        return baseItem?.name;
    }

    public override int GetCurrencyValue()
    {
        return baseItem.currencyValue;
    }

    public override bool AreEqual(Item otherItem)
    {
        if (otherItem is ReferenceItem)
            if (((ReferenceItem)otherItem).baseItem == baseItem)
                return true;

        return false;
    }
}