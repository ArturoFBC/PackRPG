using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SpeciesSpawnRate
{
    public Species species;
    public float spawnRate;
}

[System.Serializable]
public struct ItemDropRate
{
    public Droppable drop;
    public float rate;
}

[CreateAssetMenu(menuName = "Area/Area")]
public class Area : ScriptableObject
{
    [System.Serializable]
    private class DropRateDrawable
    {
        public BaseMaterialItem materialItem;
        public BaseRecipe recipe;
        public Species species;
        public float rate;

        public Droppable GetDroppable()
        {
            if ( materialItem != null )
            {
                DroppableItem droppableItem = (DroppableItem)CreateInstance("DroppableItem");
                droppableItem.SetBaseItem(materialItem);
                return droppableItem;
            }
            else if ( recipe != null )
            {
                return recipe;
            }
            else if ( species != null )
            {
                return new Essence(species);
            }
            return null;
        }
    }

    public SceneReference scene;

    public int areaLevel;

    [Header("Creatures")]
    public List<SpeciesSpawnRate> species;

    [Header("Items")]
    [SerializeField] private List<DropRateDrawable> items;

    public Vector3 startingPosition;
    public Vector3 exitToNextAreaPosition;
    public Area nextArea;

    public List<ItemDropRate> GetDropRates()
    {
        List<ItemDropRate> returnList = new List<ItemDropRate>();

        foreach ( DropRateDrawable drawable in items )
        {
            ItemDropRate dropRate = new ItemDropRate()
            {
                drop = drawable.GetDroppable(),
                rate = drawable.rate
            };
            returnList.Add(dropRate);
        }

        return returnList;
    }
}
