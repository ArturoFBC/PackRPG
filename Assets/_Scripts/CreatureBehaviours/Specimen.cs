using System;
using System.Collections.Generic;
using UnityEngine;

public enum BaseStatSource
{
    SPECIES,
    GENOTYPE,
    PHENOTYPE,
    ITEMS
}

[System.Serializable]
public class SpecimenData
{
    public int speciesIndex;

    public string name;
    public float exp;

    // STATS
    public BonusPrimaryStats genotype;
    public BonusPrimaryStats phenotype;

    // SKILLS
    public int passive_1;
    public int passive_2;
    public int passive_3;

    public int basicAttack;
    public int cooldownAttack_1;
    public int cooldownAttack_2;
    public int ultimateAttack;

    public List<int> skillsToLearn = new List<int>();

    // EQUIPMENT
    public List<ItemSaveData> equipmentItemIndex = new List<ItemSaveData>();

    public SpecimenData(Specimen specimen)
    {
        speciesIndex = ScriptableReferencesHolder.GetSpeciesIndex(specimen.species);

        name = specimen.name;
        exp  = specimen.exp;

        genotype  = new BonusPrimaryStats( specimen.genotype );
        phenotype = new BonusPrimaryStats( specimen.phenotype );

        passive_1 = ScriptableReferencesHolder.GetSkillIndex(specimen.passive_1);
        passive_2 = ScriptableReferencesHolder.GetSkillIndex(specimen.passive_2);
        passive_3 = ScriptableReferencesHolder.GetSkillIndex(specimen.passive_3);

        basicAttack = ScriptableReferencesHolder.GetSkillIndex(specimen.basicAttack);
        cooldownAttack_1 = ScriptableReferencesHolder.GetSkillIndex(specimen.cooldownAttack_1);
        cooldownAttack_2 = ScriptableReferencesHolder.GetSkillIndex(specimen.cooldownAttack_2);
        ultimateAttack = ScriptableReferencesHolder.GetSkillIndex(specimen.ultimateAttack);

        foreach (BaseSkill baseSkill in specimen.skillsToLearn)
            skillsToLearn.Add(ScriptableReferencesHolder.GetSkillIndex(baseSkill));

        foreach (EquipmentSlot equipmentSlot in specimen.equipmentSlots)
            equipmentItemIndex.Add((equipmentSlot.item == null ? null : equipmentSlot.item.GetSaveData()));

        Debug.Log(equipmentItemIndex.Count);
    }
}


[Serializable]
public class Specimen
{
    public Species species;

    public string name = "Unnamed";
    public float exp;
    public Color secondaryColor = Color.clear;

    // STATS
    public BonusPrimaryStats genotype = new BonusPrimaryStats();
    public BonusPrimaryStats phenotype = new BonusPrimaryStats();

    // SKILLS
    public BasePassiveSkill passive_1;
    public BasePassiveSkill passive_2;
    public BasePassiveSkill passive_3;

    public BaseActiveSkill basicAttack;
    public BaseActiveSkill cooldownAttack_1;
    public BaseActiveSkill cooldownAttack_2;
    public BaseActiveSkill ultimateAttack;

    public List<BaseSkill> skillsToLearn = new List<BaseSkill>();

    // EQUIPMENT
    public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();

    public delegate void EquipmentChanged(Item item, int slotIndex);
    public event EquipmentChanged EquipmentChangedEvent;

    public delegate void StatsChanged(Dictionary<BaseStatSource, float[]> stats);
    public event StatsChanged StatsChangedEvent;


    public Specimen()
    {
        LinkEvents();
    }

    // Initialize specimen from data loaded from disk (data from a saved game)
    public Specimen( SpecimenData data )
    {
        species = ScriptableReferencesHolder.GetSpeciesReference(data.speciesIndex);

        name = data.name;
        exp = data.exp;

        genotype = new BonusPrimaryStats(data.genotype);
        phenotype = new BonusPrimaryStats(data.phenotype);

        passive_1 = ScriptableReferencesHolder.GetSkillReference(data.passive_1) as BasePassiveSkill;
        passive_2 = ScriptableReferencesHolder.GetSkillReference(data.passive_2) as BasePassiveSkill;
        passive_3 = ScriptableReferencesHolder.GetSkillReference(data.passive_3) as BasePassiveSkill;

        basicAttack =      ScriptableReferencesHolder.GetSkillReference(data.basicAttack)      as BaseActiveSkill;
        cooldownAttack_1 = ScriptableReferencesHolder.GetSkillReference(data.cooldownAttack_1) as BaseActiveSkill;
        cooldownAttack_2 = ScriptableReferencesHolder.GetSkillReference(data.cooldownAttack_2) as BaseActiveSkill;
        ultimateAttack =   ScriptableReferencesHolder.GetSkillReference(data.ultimateAttack)   as BaseActiveSkill;

        foreach (int skillIndex in data.skillsToLearn)
        {
            BaseSkill retrievedSkill = ScriptableReferencesHolder.GetSkillReference(skillIndex);
            if ( retrievedSkill != null )
                skillsToLearn.Add(retrievedSkill);
        }

        // Create item slots and see that the items loaded from the save file match the item slot type
        Debug.Log(data.equipmentItemIndex.Count);
        for (int i = 0; i < species.equipmentSlots.Length; i++)
        {
            EquipmentSlot newSlot = new EquipmentSlot();
            newSlot.type = species.equipmentSlots[i];

            if (data.equipmentItemIndex.Count <= i ||
                data.equipmentItemIndex[i] == null)
                newSlot.item = null;
            else
                newSlot.item = ItemFactory.CreateItem(data.equipmentItemIndex[i]);

            equipmentSlots.Add(newSlot);
        }

        LinkEvents();
    }

    ~Specimen()
    {
        UnlinkEvents();
    }

    #region EVENTS
    private void LinkEvents()
    {
        genotype.StatsChangedEvent += OnStatChanged;
        phenotype.StatsChangedEvent += OnStatChanged;
        EquipmentChangedEvent += OnEquipmentChange;
    }

    private void OnEquipmentChange(Item item, int slotIndex)
    {
        StatsChangedEvent?.Invoke(GetStatsPerCategory());
    }

    private void OnStatChanged(float[] stats)
    {
        StatsChangedEvent?.Invoke(GetStatsPerCategory());
    }

    private void UnlinkEvents()
    {
        genotype.StatsChangedEvent -= OnStatChanged;
        phenotype.StatsChangedEvent -= OnStatChanged;
        EquipmentChangedEvent -= OnEquipmentChange;
    }
    #endregion

    public void SetGenotype( float fractionOfMaxGenotype )
    {
        genotype.SetStatsFractionOfMax(fractionOfMaxGenotype);
    }

    public void RemoveSkillFromTheSkillsToLearnList(BaseSkill skill)
    {
        if (skillsToLearn.Contains(skill))
            skillsToLearn.Remove(skill);
    }

    public Item EquipItem(Item item, EquipmentSlot slot )
    {
        if (!equipmentSlots.Contains(slot))
            return null;

        Item unequippedItem = null;
        if ( slot.item != null )
            unequippedItem = UnequipItem(slot);

        InventoryManager.Ref.RemoveItem(item);
        slot.item = item;

        EquipmentChangedEvent?.Invoke(item, equipmentSlots.IndexOf(slot));

        return unequippedItem;
    }

    public Item UnequipItem(EquipmentSlot slot)
    {
        if (!equipmentSlots.Contains(slot))
            return null;

        Item unequippedItem = slot.item;

        InventoryManager.Ref.AddItem(slot.item);
        slot.item = null;

        EquipmentChangedEvent?.Invoke(null, equipmentSlots.IndexOf(slot));

        return unequippedItem;
    }

    #region STAT ACCESS
    public float[] GetTotalStats()
    {
        float[] returnStats = (float[])species.baseStats.Clone();
        float[] itemStats = GetItemStats();

        for ( int i = 0; i < Enum.GetValues(typeof(PrimaryStat)).Length; i++ )
            returnStats[i] += genotype[i] + phenotype[i] + itemStats[i];

        return returnStats;
    }

    public Dictionary<BaseStatSource,float[]> GetStatsPerCategory()
    {
        Dictionary<BaseStatSource, float[]> returnStats = new Dictionary<BaseStatSource, float[]>();

        int level = Level.CalculateLevel(exp);
        returnStats.Add( BaseStatSource.SPECIES,   RolePlayingFormulas.ApplyLevelToBaseStats( (float[])species.baseStats.Clone(), level ));
        returnStats.Add( BaseStatSource.GENOTYPE,  RolePlayingFormulas.ApplyLevelToBaseStats( genotype.GetStats(),                level ));
        returnStats.Add( BaseStatSource.PHENOTYPE, RolePlayingFormulas.ApplyLevelToBaseStats( phenotype.GetStats(),               level ));
        returnStats.Add( BaseStatSource.ITEMS, GetItemStats());

        return returnStats;
    }

    public float[] GetItemStats()
    {
        // Get all info from the items
        List<float[]> itemsStats = new List<float[]>();
        foreach ( EquipmentSlot equipmentSlot in equipmentSlots)
        {
            if ( ( equipmentSlot.type == EquipmentType.CHARM) &&
                 ( equipmentSlot.item != null ) &&
                 ( equipmentSlot.item is StatModifierItem) )
            {
                StatModifierItem statModifierItem = (StatModifierItem)equipmentSlot.item;

                itemsStats.Add( statModifierItem.GetExtraStats() );
            }
        }

        // Condense the information from the items
        int statAmount = Enum.GetValues(typeof(PrimaryStat)).Length;
        float[] returnStats = new float[statAmount];
        for (int i = 0; i < statAmount; i++)
        {
            foreach ( float[] iS in itemsStats)
            {
                returnStats[i] += iS[i];
            }
        }

        return returnStats;
    }
    #endregion

    public void ApplyMorph( Morph targetMorph )
    {
        species = targetMorph.targetSpecies;

        PackpediaManager.NotifyOwnedSpecies(targetMorph.targetSpecies);
    }
}
