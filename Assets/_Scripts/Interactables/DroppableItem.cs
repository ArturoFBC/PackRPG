using UnityEngine;

[System.Serializable]
public class DroppableItem : Droppable
{
    [SerializeField] private BaseItem myItem;

    public DroppableItem(BaseItem baseItem)
    {
        SetBaseItem(baseItem);
    }

    public void SetBaseItem( BaseItem baseItem )
    {
        myItem = baseItem;
        dropColor = baseItem.dropColor;
        dropMesh = baseItem.dropMesh;
    }

    public override void AddToInventory(int amount)
    {
        InventoryManager.Ref.AddItem(new ReferenceItem(myItem), amount);
    }

    public override string GetName()
    {
        return TextReferences.GetText( myItem.name );
    }
}