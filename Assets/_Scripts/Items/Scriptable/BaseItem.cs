using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    CONSUMABLE,
    EQUIPMENT,
    MATERIAL,
    ESSENCE
}

[System.Serializable]
public abstract class BaseItem : ScriptableObject
{
    [Header ("ItemProperties")]
    public string description;
    public Sprite icon;
    public int currencyValue;

    [Header("Droppable properties")]
    public Color dropColor;
    public Mesh dropMesh;

    public abstract ItemType GetItemType();
}
