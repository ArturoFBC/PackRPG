using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSaveButton : MonoBehaviour
{
    public void DeleteSave( int saveSlot )
    {
        SaveLoadManager.ClearData((uint)saveSlot);
    }
}
