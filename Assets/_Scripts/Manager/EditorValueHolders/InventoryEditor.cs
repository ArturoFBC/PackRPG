using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game progress to be used for testing while in the editor
[CreateAssetMenu(menuName = "Items/InventoryEditor")]
public class InventoryEditor : ScriptableObject
{
    public int _GeneralEssence;
    public EssenceInventory _SpecificEssences;
    public List<InventoryEntry> _Items;
}
