using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essence : Droppable, IScrapable
{
    const int ESSENCE_TO_CURRENCY_RATIO = 1;

    private Species species;

    private string name;

    public Species GetSpecies()
    {
        return species;
    }

    public Essence(Species species)
    {
        this.species = species;
        dropColor = Color.yellow;
        name = species.name + " essence";
        autoPickUp = true;
    }

    public static List<Essence> GetEssences( Species targetSpecies )
    {
        List<Species> baseSpecies = Species.GetOriginalSpecies(targetSpecies);

        List<Essence> returnEssences = new List<Essence>();
        foreach (Species species in baseSpecies)
        {
            Essence newEssence = new Essence(species);
            returnEssences.Add(newEssence);
        }
        return returnEssences;
    }

    public override void AddToInventory(int amount)
    {
        InventoryManager.Ref.AddEssence(species, amount);
    }

    public override string GetName()
    {
        return name.Split('_')[1];
    }

    public int GetInventoryAmount()
    {
        return InventoryManager.Ref.GetEssenceAmount(species);
    }

    public int GetCurrencyValue()
    {
        return ESSENCE_TO_CURRENCY_RATIO;
    }

    public void Scrap(int amount)
    {
        InventoryManager.Ref.SpendEssence(species, amount);
        InventoryManager.Ref.AddEssence(amount * ESSENCE_TO_CURRENCY_RATIO);
    }

    public Sprite GetIcon()
    {
        return species.miniature;
    }
}
