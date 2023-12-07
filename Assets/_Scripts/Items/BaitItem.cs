using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaitItem : ConsumableItem
{
    Dictionary<PrimaryStat, float> statIncreases = new Dictionary<PrimaryStat, float>();

    private const string baitImagePath = "BaitIcon";

    Species speciesThisBaitIsFor;

    private Sprite icon;

    public BaitItem( Species species, Dictionary<PrimaryStat,float> stats )
    {
        speciesThisBaitIsFor = species;
        statIncreases = stats;
    }

    public BaitItem(ItemSaveData saveData) : base(saveData)
    {
        foreach (KeyValuePair<int, float> statIncrease in saveData.statsIncreased)
            statIncreases.Add((PrimaryStat)statIncrease.Key, statIncrease.Value);

        speciesThisBaitIsFor = ScriptableReferencesHolder.GetSpeciesReference( saveData.speciesIndex );
    }

    public override ItemSaveData GetSaveData()
    {
        ItemSaveData saveData = base.GetSaveData();

        foreach (KeyValuePair<PrimaryStat, float> statIncrease in statIncreases)
            saveData.statsIncreased.Add((int)statIncrease.Key, statIncrease.Value);

        saveData.speciesIndex = ScriptableReferencesHolder.GetSpeciesIndex(speciesThisBaitIsFor);

        return saveData;
    }

    public override string GetDescription()
    {
        return TextReferences.GetText("@bait_description");
    }

    public override Sprite GetIcon()
    {
        if (icon == null)
            icon = CreateIcon();

        return icon;
    }

    private Sprite CreateIcon()
    {
        List<LayerTextures.LayerParams> layerParams = new List<LayerTextures.LayerParams>();

        layerParams.Add(new LayerTextures.LayerParams
        {
            texture = speciesThisBaitIsFor.miniature.texture,
            size = new Vector2(0.85f, 0.85f),
            offset = new Vector2(0f, 0.15f)
        }); ;

        layerParams.Add(new LayerTextures.LayerParams
        {
            texture = Resources.Load<Texture2D>(baitImagePath),
            size = new Vector2(0.5f, 0.5f),
            offset = new Vector2(0.5f, 0f)
        });

        Vector2Int size = new Vector2Int(layerParams[0].texture.width, layerParams[0].texture.height);

        return LayerTextures.GetLayeredTexture(layerParams, size);
    }

    public override string GetName()
    {
        string name = "";

        foreach (KeyValuePair<PrimaryStat, float> increase in statIncreases)
            name += increase.Key.ToString() + " ";

        name += TextReferences.GetText("bait_name");

        return name;
    }

    public override bool AreEqual(Item otherItem)
    {
        if (!(otherItem is BaitItem))
            return false;

        BaitItem other = (BaitItem)otherItem;

        if (other == this)
            return true;


        if ( GetItemType()          == other.GetItemType()  &&
             tier                   == other.tier           &&
             speciesThisBaitIsFor   == other.GetSpecies()   &&
             statIncreases.Count    == other.statIncreases.Count && !statIncreases.Except(other.statIncreases).Any() )
        {
            return true;
        }

        return false;
    }

    public Species GetSpecies()
    {
        return speciesThisBaitIsFor;
    }

    public override TargetType GetTargetType()
    {
        return TargetType.ENEMY;
    }

    public override void UseItem(GameObject targetObject)
    {
        if (targetObject.tag == "Enemy")
        {
            CreatureStats targetStats = targetObject.GetComponent<CreatureStats>();
            if (targetStats != null)
            {
                // CHECK THAT THE BAIT IS THE CORRECT SPECIES FOR THIS PARTICULAR ENEMY
                if (Species.GetOriginalSpecies(targetStats.GetSpecimen().species).Contains(speciesThisBaitIsFor))
                {
                    Baitable enemyBaitable = targetObject.GetComponent<Baitable>();
                    if (enemyBaitable != null)
                    { 
                        enemyBaitable.BaitCreature(statIncreases);
                        InventoryManager.Ref.RemoveItem(this);
                    }
                }
                else
                {
                    Debug.LogError("Creature is of invalid species");
                }
            }
        }
    }
}
