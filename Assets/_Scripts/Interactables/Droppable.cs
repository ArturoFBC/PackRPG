using UnityEngine;

[System.Serializable]
public abstract class Droppable : ScriptableObject
{
    [Header("Droppable properties")]
    public Color dropColor;
    public Mesh dropMesh;
    public bool autoPickUp;

    public abstract void AddToInventory(int amount);

    public abstract string GetName();
}