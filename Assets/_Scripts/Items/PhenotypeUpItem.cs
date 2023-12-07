using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhenotypeUpItem : ConsumableItem
{
    private PrimaryStat myStat;


    public PhenotypeUpItem(PrimaryStat stat)
    {
        myStat = stat;
    }

    public PhenotypeUpItem(ItemSaveData saveData) : base(saveData)
    {
        myStat = (PrimaryStat)saveData.statIncrease;
    }

    public override ItemSaveData GetSaveData()
    {
        ItemSaveData saveData = base.GetSaveData();

        saveData.statIncrease = (int)myStat;

        return saveData;
    }

    public override bool AreEqual(Item otherItem)
    {
        if (!(otherItem is PhenotypeUpItem))
            return false;

        PhenotypeUpItem other = (PhenotypeUpItem)otherItem;

        if (other == this)
            return true;


        if (GetItemType() == other.GetItemType() &&
             tier == other.tier &&
             myStat == other.myStat)
        {
            return true;
        }

        return false;
    }

    public override string GetDescription()
    {
        return TextReferences.GetText("@pill_description");
    }

    public override Sprite GetIcon()
    {
        throw new System.NotImplementedException();
    }

    public override string GetName()
    {
        string name = myStat.ToString() + " " + TextReferences.GetText("pill_name");
        return name;
    }

    public override TargetType GetTargetType()
    {
        return TargetType.ALLY;
    }

    public override void UseItem(GameObject targetObject)
    {
        CreatureStats targetStats = targetObject.GetComponent<CreatureStats>();
        if (targetStats != null)
        {
            Specimen targetSpecimen = targetStats.GetSpecimen();
            if ( targetSpecimen != null )
            {
                targetSpecimen.phenotype.GainStat(myStat, (1 + tier) / 5f);
            }
        }
    }
}