using System;
using System.Collections.Generic;
using UnityEngine;

public class StatModifierItem : Item
{
    static readonly float[] tierStatIncreaseBaseValues = { 10f, 20f, 40f, 65f, 100f };

    private const string baseCharmImagePath = "CharmIcon/charm_1_base";
    private const string leftCharmImagePath = "CharmIcon/charm_1_L";
    private const string rightCharmImagePath = "CharmIcon/charm_1_R";


    private Sprite icon;

    public List<PrimaryStat> statsIncreased = new List<PrimaryStat>();
    public PrimaryStat extraStat;
    public float extraStatStrength;


    public StatModifierItem(ItemSaveData saveData) : base(saveData)
    {
        foreach (int i in saveData.mainStatsIncreased)
            statsIncreased.Add((PrimaryStat)i);

        extraStat = (PrimaryStat)saveData.secondaryStatIncreased;
        extraStatStrength = saveData.secondaryStatStrength;
    }

    public StatModifierItem(int tier, List<PrimaryStat> statsIncreased, PrimaryStat extraStat, float extraStatStrength)
    {
        type = ItemType.EQUIPMENT;
        equipmentType = EquipmentType.CHARM;

        this.tier = tier;
        this.statsIncreased = new List<PrimaryStat>(statsIncreased);
        this.extraStat = extraStat;
        this.extraStatStrength = extraStatStrength;
    }

    public StatModifierItem(int tier, List<PrimaryStat> statsIncreased)
    {
        type = ItemType.EQUIPMENT;
        equipmentType = EquipmentType.CHARM;

        this.tier = tier;
        this.statsIncreased = new List<PrimaryStat>(statsIncreased);
        this.extraStat = (PrimaryStat)UnityEngine.Random.Range((int)PrimaryStat.HP, (int)PrimaryStat.MP);
        this.extraStatStrength = UnityEngine.Random.value;
    }

    public override ItemSaveData GetSaveData()
    {
        ItemSaveData saveData = base.GetSaveData();

        saveData.secondaryStatIncreased = (int)extraStat;
        saveData.secondaryStatStrength = extraStatStrength;

        foreach (PrimaryStat ps in statsIncreased)
            saveData.mainStatsIncreased.Add((int)ps);

        return saveData;
    }

    public List<StatModifier> GetStatModifiers()
    {
        List<StatModifier> modifierList = new List<StatModifier>();

        foreach ( PrimaryStat ps in statsIncreased)
        {
            float statValue = GetMainStatsValue( statsIncreased.Count, tierStatIncreaseBaseValues[base.tier] );
            modifierList.Add( CreateStatModifier( ps, statValue ));
        }

        float extraStatValue = GetExtraStatValue( extraStatStrength, tierStatIncreaseBaseValues[base.tier] );
        modifierList.Add( CreateStatModifier(extraStat, extraStatValue ));

        return modifierList;
    }

    public float[] GetExtraStats()
    {
        int statNumber = Enum.GetValues(typeof(PrimaryStat)).Length;
        float[] returnStats = new float[statNumber];

        for (int i = 0; i < statsIncreased.Count; i++)
        {
            returnStats[(int)statsIncreased[i]] += GetMainStatsValue( statsIncreased.Count, tierStatIncreaseBaseValues[tier]);
        }
        returnStats[(int)extraStat] += GetExtraStatValue(extraStatStrength, tierStatIncreaseBaseValues[tier]);

        return returnStats;
    }

    private StatModifier CreateStatModifier( PrimaryStat stat, float value )
    {
        StatModifier newStatModifier = new StatModifier
        {
            value = value,
            stat = (BuffType)stat,
            operation = StatModifier.ModifierOperation.ADDITIVE,
            maxStacks = 1,
            timed = false,
            stacks = 1
        };

        return newStatModifier;
    }

    private float GetMainStatsValue( int statAmount, float baseValue )
    {
        return baseValue * ( ( statAmount + 2f ) / ( statAmount * 3f ));
    }

    private float GetExtraStatValue( float extraStatValue, float baseValue)
    {
        return baseValue * (0.25f + (0.25f * extraStatValue));
    }

    public override string GetName()
    {
        string name = "Charm";

        foreach (PrimaryStat stat in statsIncreased)
            name += " " + stat.ToString();

        return name;
    }

    public override string GetDescription()
    {
        string description = "";

        foreach (PrimaryStat stat in statsIncreased)
            description += "+" + (int)GetMainStatsValue(statsIncreased.Count, tierStatIncreaseBaseValues[base.tier]) + " " + stat.ToString() + "\n";

        description += "+" + (int)GetExtraStatValue(extraStatStrength, tierStatIncreaseBaseValues[base.tier]) + " " + extraStat.ToString() + "\n";

        return description;
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
            texture = Resources.Load<Texture2D>(baseCharmImagePath),
            color = default
        });

        layerParams.Add(new LayerTextures.LayerParams
        {
            texture = Resources.Load<Texture2D>(leftCharmImagePath),
            color = IconsAndEffects._Ref.statColors[(int)statsIncreased[0]]
        });

        layerParams.Add(new LayerTextures.LayerParams
        {
            texture = Resources.Load<Texture2D>(rightCharmImagePath),
            color = statsIncreased.Count > 1 ? IconsAndEffects._Ref.statColors[(int)statsIncreased[1]] : IconsAndEffects._Ref.statColors[(int)statsIncreased[0]]
        });

        Vector2Int size = new Vector2Int(layerParams[0].texture.width, layerParams[0].texture.height);

        return LayerTextures.GetLayeredTexture( layerParams, size );
    }

    public static Sprite GetNeutralIcon()
    {
        List<LayerTextures.LayerParams> layerParams = new List<LayerTextures.LayerParams>();

        layerParams.Add(new LayerTextures.LayerParams
        {
            texture = Resources.Load<Texture2D>(baseCharmImagePath),
            color = default
        });

        layerParams.Add(new LayerTextures.LayerParams
        {
            texture = Resources.Load<Texture2D>(leftCharmImagePath),
            color = Color.gray
        });

        layerParams.Add(new LayerTextures.LayerParams
        {
            texture = Resources.Load<Texture2D>(rightCharmImagePath),
            color = Color.gray
        });

        Vector2Int size = new Vector2Int(layerParams[0].texture.width, layerParams[0].texture.height);

        return LayerTextures.GetLayeredTexture(layerParams, size);
    }

    public override ItemType GetItemType()
    {
        return ItemType.EQUIPMENT;
    }

    public override int GetCurrencyValue()
    {
        return (int)tierStatIncreaseBaseValues[tier];
    }

    public override bool AreEqual(Item otherItem)
    {
        if (otherItem is StatModifierItem)
        {
            StatModifierItem other = (StatModifierItem)otherItem;

            if ( tier == other.tier &&
                 extraStat == other.extraStat &&
                 extraStatStrength == other.extraStatStrength &&
                 Equals( statsIncreased, other.statsIncreased ) )
                return true;
        }

        return false;
    }
}
