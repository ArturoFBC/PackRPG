using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoadManager
{
    public const uint _SaveSlotAmount = 3;
    static string _SaveFileName = "savegame{0}.data";

    static bool _Loaded = false;
    static SaveData[] _SaveStates = new SaveData[_SaveSlotAmount];
    static uint _SelectedSlot;
    public static uint selectedSlot
    {
        get { return _SelectedSlot; }
        set { if (value < _SaveSlotAmount) _SelectedSlot = value; }
    }

    public static void LoadAllStates()
    {
        for (uint stateIndex = 0; stateIndex < _SaveSlotAmount; stateIndex++)
        {
            _SaveStates[stateIndex] = Load(stateIndex);
        }
        _Loaded = true;
    }

    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetSavePath(_SelectedSlot);
        FileStream stream = new FileStream( path, FileMode.Create );

        _SaveStates[_SelectedSlot] = new SaveData();

        formatter.Serialize(stream, _SaveStates[_SelectedSlot]);
        stream.Close();

        Debug.Log("Saved to file " + path);
    }

    public static SaveData GetSave()
    {
        if (!_Loaded)
            LoadAllStates();

        return _SaveStates[_SelectedSlot];
    }

    public static void ClearData( uint index )
    {
        _SaveStates[index] = null;

        string path = GetSavePath(index);

        if (File.Exists(path))
            File.Delete(path);
    }

    static SaveData Load(uint index)
    {
        string path = GetSavePath(index);

        if ( File.Exists(path) )
        {
            Debug.Log("Save file exists in " + path);

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Save file does not exist in " + path);

            return null;
            // TO DO Create file
        }
    }

    static string GetSavePath(uint index)
    {
        return Application.persistentDataPath + "/" + string.Format(_SaveFileName, index);
    }
}
