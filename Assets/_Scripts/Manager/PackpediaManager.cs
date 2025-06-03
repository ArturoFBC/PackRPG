using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveLoad;

public class PackpediaManager : Singleton<PackpediaManager>, ISaveable
{
    public List<Species> _OwnedSpecies = new List<Species>();

    public static List<Species> ownedSpecies
    {
        get { return Ref._OwnedSpecies; }
    }

    public static void Load(PackpediaData data)
    {
        Ref.Reset();

        Ref._OwnedSpecies = new List<Species>();
        foreach (int speciesIndex in data.ownedSpecies)
            ownedSpecies.Add(ScriptableReferencesHolder.GetSpeciesReference(speciesIndex));
    }

    public void Reset()
    {
        Ref._OwnedSpecies = new List<Species>();
    }

    public static void NotifyOwnedSpecies(Species speciesOwned)
    {
        if (Ref._OwnedSpecies.Contains(speciesOwned) == false)
            Ref._OwnedSpecies.Add(speciesOwned);
    }

    public static bool IsSpeciesOwned( Species speciesQueried )
    {
        return Ref._OwnedSpecies.Contains(speciesQueried);
    }
}
