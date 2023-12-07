using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreatureStorageData
{
    public List<SpecimenData> _ActivePack = new List<SpecimenData>();
    public List<SpecimenData> _StoredCreatures = new List<SpecimenData>();

    public CreatureStorageData( CreatureStorage creatureStorage )
    {
        foreach (Specimen specimen in creatureStorage._ActivePack)
            _ActivePack.Add(new SpecimenData(specimen));

        foreach (Specimen specimen in creatureStorage._StoredCreatures)
            _StoredCreatures.Add(new SpecimenData(specimen));
    }
}

[System.Serializable]
public class GameProgressData
{
    public Dictionary<int, bool> _AreaStates = new Dictionary<int, bool>();

    public GameProgressData(GameProgress gameProgress)
    {
        foreach (AreaUnlock areaUnlock in gameProgress.GetAreaList())
            _AreaStates.Add(ScriptableReferencesHolder.GetAreaIndex(areaUnlock.area), areaUnlock.unlocked);
    }
}

[System.Serializable]
public class InventoryManagerData
{
    public int _GeneralEssence = 0;
    public Dictionary<ItemSaveData, int> _Items = new Dictionary<ItemSaveData, int>();
    public Dictionary<int, int> _SpecificEssences = new Dictionary<int, int>();
    public List<int> _UnlockedRecipes = new List<int>();

    public InventoryManagerData(InventoryManager inventoryManager)
    {
        _GeneralEssence = inventoryManager.GetEssence();

        foreach (InventoryEntry ii in inventoryManager._Items)
            _Items.Add(ii.item.GetSaveData(), ii.amount);

        foreach (EssenceValue essenceValue in inventoryManager._SpecificEssences)
            _SpecificEssences.Add(ScriptableReferencesHolder.GetSpeciesIndex(essenceValue.species), essenceValue.amount);

        foreach (BaseRecipe recipe in inventoryManager._Recipes)
            _UnlockedRecipes.Add(ScriptableReferencesHolder.GetRecipeIndex(recipe));
    }
}

[System.Serializable]
public class PackpediaData
{
    public List<int> ownedSpecies = new List<int>();

    public PackpediaData(PackpediaManager packpediaManager)
    {
        foreach (Species species in packpediaManager._OwnedSpecies)
            ownedSpecies.Add(ScriptableReferencesHolder.GetSpeciesIndex(species));
    }
}

[System.Serializable]
public class SaveData
{
    public CreatureStorageData creatureStorageData;
    public GameProgressData gameProgressData;
    public InventoryManagerData InventoryManagerData;
    public PackpediaData packpediaData;

    public SaveData()
    {
        creatureStorageData = new CreatureStorageData(CreatureStorage.Ref);
        gameProgressData = new GameProgressData(GameProgress.Ref);
        InventoryManagerData = new InventoryManagerData(InventoryManager.Ref);
        packpediaData = new PackpediaData(PackpediaManager.Ref);
    }
}
