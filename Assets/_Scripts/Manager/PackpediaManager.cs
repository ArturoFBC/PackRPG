using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackpediaManager : Singleton<PackpediaManager>
{
    public List<Species> _OwnedSpecies = new List<Species>();

    public static List<Species> ownedSpecies
    {
        get { return Ref._OwnedSpecies; }
    }

    public static void Load(PackpediaData data)
    {
        Reset();

        Ref._OwnedSpecies = new List<Species>();
        foreach (int speciesIndex in data.ownedSpecies)
            ownedSpecies.Add(ScriptableReferencesHolder.GetSpeciesReference(speciesIndex));
    }

    public static void Reset()
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
