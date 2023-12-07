using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game progress to be used for testing while in the editor
[CreateAssetMenu(menuName = "Creatures/CreatureStorageEditor")]
public class CreatureStorageEditor : ScriptableObject
{
    public List<Specimen> activePack;
    public List<Specimen> storedCreatures;
}
