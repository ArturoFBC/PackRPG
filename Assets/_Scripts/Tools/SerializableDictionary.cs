using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializablePair<TKey, TValue>
{
    [SerializeField]
    public TKey key;
    [SerializeField]
    public TValue value;

    public SerializablePair(TKey define_key, TValue define_value )
    {
        key = define_key;
        value = define_value;
    }
}


[System.Serializable]
public class SerializableDictionary<TKey, TValue> where TKey : class
{
    [SerializeField]
    private List<SerializablePair<TKey, TValue>> pairList = new List<SerializablePair<TKey, TValue>>();

    public TValue this[TKey keyIndex]
    {
        get { return pairList.Find( x => x.key == keyIndex ).value; }
        set
        {
            for (int i = 0; i < pairList.Count; i++)
            {
                if (pairList[i].key == keyIndex)
                {
                    pairList[i].value = value;
                    return;
                }
            }
            throw new System.IndexOutOfRangeException("The key used as essence key was not found on the list of essences of the inventory");
        }
    }

    public void Clear()
    {
        pairList.Clear();
    }

    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key))
        {
            throw new System.ArgumentException("Essence type already exists in the inventory");
        }
        else
        {
            SerializablePair<TKey, TValue> newEssenceValue = new SerializablePair<TKey, TValue>(key, value);
            pairList.Add(newEssenceValue);
        }
    }

    public bool Remove(TKey keyToRemove)
    {
        return pairList.Remove(pairList.Find(x => x.key == keyToRemove));
    }

    public bool ContainsKey(TKey key)
    {
        SerializablePair<TKey, TValue> foundEssence = pairList.Find(x => x.key == key);

        return (foundEssence != null);
    }
}