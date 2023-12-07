using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SerializableDictionaryTwo : ISerializationCallbackReceiver
{
    [SerializeField]
    public List<Species> _keys = new List<Species>();
    [SerializeField]
    public List<int> _values = new List<int>();

    //Unity doesn't know how to serialize a Dictionary
    public Dictionary<Species, int> _myDictionary = new Dictionary<Species, int>();

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (var kvp in _myDictionary)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        _myDictionary = new Dictionary<Species, int>();

        for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            _myDictionary.Add(_keys[i], _values[i]);
    }
}
