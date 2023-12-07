using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EssenceValue
{
    public Species species;
    public int amount;

    public EssenceValue( Species define_species, int define_amount = 0)
    {
        species = define_species ?? throw new System.ArgumentNullException("The species is used as an index and as such can not be null");
        amount = define_amount;
    }
}

//
// The whole class is a list wrapped as a dictionary so it appears in the inspector
//
[System.Serializable]
public class EssenceInventory : IEnumerable, IEnumerator
{
    [SerializeField]
    private List<EssenceValue> essences = new List<EssenceValue>();

    public int Count
    {
        get { return essences.Count; }
        private set { }
    }

    object IEnumerator.Current => ((IEnumerator)essences).Current;

    public int this[Species speciesIndex]
    {
        get { return essences.Find(x => x.species == speciesIndex).amount; }
        set
        {
            for (int i = 0; i < essences.Count; i++)
            {
                if (essences[i].species == speciesIndex)
                {
                    essences[i].amount = value;
                    return;
                }
            }
            throw new System.IndexOutOfRangeException( "The species used as essence key was not found on the list of essences of the inventory" );
        }
    }

    public void Clear()
    {
        essences.Clear();
    }

    public void Add ( Species species, int amount )
    {
        if ( ContainsKey(species) )
        {
            throw new System.ArgumentException("Essence type already exists in the inventory");
        }
        else
        {
            EssenceValue newEssenceValue = new EssenceValue(species, amount);
            essences.Add(newEssenceValue);
        }
    }

    public bool Remove( Species speciesToRemove )
    {
        return essences.Remove(essences.Find(x => x.species == speciesToRemove));
    }

    public bool ContainsKey( Species species )
    {
        EssenceValue foundEssence = essences.Find( x => x.species == species );

        return (foundEssence != null);
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable)essences).GetEnumerator();
    }

    bool IEnumerator.MoveNext()
    {
        return ((IEnumerator)essences).MoveNext();
    }

    void IEnumerator.Reset()
    {
        ((IEnumerator)essences).Reset();
    }
}