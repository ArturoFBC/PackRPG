using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureStorage : Singleton<CreatureStorage>
{
    public List<Specimen> _ActivePack = new List<Specimen>();
    public List<Specimen> _StoredCreatures = new List<Specimen>();

    public static List<Specimen> activePack
    {
        get { return Ref._ActivePack; }
    }
    public static List<Specimen> storedCreatures
    {
        get { return Ref._StoredCreatures; }
    }

#if UNITY_EDITOR
    // Used on the editor to fake the initialization of the data
    [SerializeField]
    private CreatureStorageEditor creatureStorageEditor;

    protected override void InheritedAwake()
    {
        _ActivePack = creatureStorageEditor.activePack;
        _StoredCreatures = creatureStorageEditor.storedCreatures;
    }
#endif

    public static void AddSpecimen(Specimen newSpecimen, Vector3 capturePosition )
    {
        if (Ref._ActivePack.Count < GameManager.MAX_PACK_SIZE)
        {
            Ref._ActivePack.Add(newSpecimen);
            GameManager.SpawnPlayerCreature(GetCreatureIndex(newSpecimen), capturePosition);
        }
        else
            Ref._StoredCreatures.Add(newSpecimen);
    }

    public static void MoveCreatureFromActivePackToStorage( int creatureIndex )
    {
        storedCreatures.Add(activePack[creatureIndex]);
        activePack.RemoveAt(creatureIndex);
    }

    public static int GetCreatureIndex(Specimen specimen)
    {
        return activePack.IndexOf(specimen);
    }

    public static void Load(CreatureStorageData data)
    {
        Ref._ActivePack = new List<Specimen>();
        foreach (SpecimenData specimen in data._ActivePack)
            activePack.Add(new Specimen(specimen));

        Ref._StoredCreatures = new List<Specimen>();
        foreach (SpecimenData specimen in data._StoredCreatures)
            storedCreatures.Add(new Specimen(specimen));
    }

    public static void Reset()
    {
        Ref._ActivePack = new List<Specimen>();
        Ref._StoredCreatures = new List<Specimen>();
    }
}
