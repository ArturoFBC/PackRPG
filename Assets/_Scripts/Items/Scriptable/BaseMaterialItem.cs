using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MaterialType
{
    Other,
    Gas,
    Metal,
    Wood,
    Crystal,
    Glue,
    Stat_core
}

[CreateAssetMenu(menuName = "Items/Material")]
public class BaseMaterialItem : BaseItem
{
    public int tier;
    public MaterialType materialType;
    public PrimaryStat stat;
    public DamageType element;

    public override ItemType GetItemType()
    {
        return ItemType.MATERIAL;
    }
}
