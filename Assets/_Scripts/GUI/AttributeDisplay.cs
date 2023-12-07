using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class AttributeDisplay : MonoBehaviour
{
    //Stats
    [SerializeField] private Text[] statLabels = new Text[8];
    [SerializeField] private DiamondGraph baseStats;
    [SerializeField] private DiamondGraph _Genotype;
    [SerializeField] private DiamondGraph _Phenotype;
    [SerializeField] private DiamondGraph _ItemStats;

    private Specimen mySpecimen;

    //Dictionary to link every stat with an 0 based index and their positions in the graph
    Dictionary<PrimaryStat, int> statConversion;
    Dictionary<PrimaryStat, int> _StatConversion
    {
        get
        {
            if (statConversion == null)
                InitializeStatConversionDictionary();

            return statConversion;
        }
    }


    private void InitializeStatConversionDictionary()
    {
        statConversion = new Dictionary<PrimaryStat, int>();
        statConversion.Add(PrimaryStat.STR, 0);
        statConversion.Add(PrimaryStat.DEF, 1);
        statConversion.Add(PrimaryStat.MP, 2);
        statConversion.Add(PrimaryStat.AGI, 3);
        statConversion.Add(PrimaryStat.INT, 4);
        statConversion.Add(PrimaryStat.WILL, 5);
        statConversion.Add(PrimaryStat.HP, 6);
        statConversion.Add(PrimaryStat.DEX, 7);
    }

    public void DisplayAttributes(Specimen specimen)
    {
        if (mySpecimen != null)
            mySpecimen.StatsChangedEvent -= DisplayAttributes;
        mySpecimen = specimen;
        mySpecimen.StatsChangedEvent += DisplayAttributes;

        DisplayAttributes(specimen.GetStatsPerCategory());
    }

    public void DisplayAttributes(Species species)
    {
        float[] stats = species.baseStats;
        DisplayAttributes( stats );
    }

    public void DisplayAttributes(Dictionary<BaseStatSource,float[]> stats)
    {
        this.enabled = true;
        float[] accumulatedStats = GetAccumulatedStats( new List<float[]>(stats.Values) );
        float hightestStat = GetHightestStat(accumulatedStats);

        FillNumberLabels(accumulatedStats);

        //Fill the graphs
        if ((_Genotype != null && _Phenotype != null && _ItemStats != null) &&
               stats.Count >= (int)BaseStatSource.ITEMS)
        {
            // If there are genotype and phenotype graphs, fill everything (full data graph)
            float[] baseStats = stats[BaseStatSource.SPECIES];
            float[] genotype = stats[BaseStatSource.GENOTYPE];
            float[] phenotype = stats[BaseStatSource.PHENOTYPE];

            for (int i = 0; i < _StatConversion.Count; i++)
            {
                this.baseStats.points[_StatConversion[(PrimaryStat)i]] = baseStats[i] / hightestStat;
                _Genotype.points[_StatConversion[(PrimaryStat)i]] = (baseStats[i] + genotype[i]) / hightestStat;
                _Phenotype.points[_StatConversion[(PrimaryStat)i]] = (baseStats[i] + genotype[i] + phenotype[i]) / hightestStat;
                _ItemStats.points[_StatConversion[(PrimaryStat)i]] = accumulatedStats[i] / hightestStat;
            }

            // Enable / disable to update
            this.baseStats.enabled = false;
            this.baseStats.enabled = true;
            _Genotype.enabled = false;
            _Genotype.enabled = true;
            _Phenotype.enabled = false;
            _Phenotype.enabled = true;
            _ItemStats.enabled = false;
            _ItemStats.enabled = true;
        }
        else
            DisplayAttributes(accumulatedStats);
    }

    public void DisplayAttributes(float[] stats)
    {
        this.enabled = true;
        float hightestStat = GetHightestStat(stats);

        FillNumberLabels(stats);

        for (int i = 0; i < _StatConversion.Count; i++)
            baseStats.points[_StatConversion[(PrimaryStat)i]] = stats[i] / hightestStat;

        baseStats.enabled = false;
        baseStats.enabled = true;
    }

    #region TOOLS
    private void FillNumberLabels(float[] stats)
    {
        //Fill text numbers if there are text numbers
        if (statLabels != null && statLabels.Length == _StatConversion.Count)
        {
            for (int i = 0; i < _StatConversion.Count; i++)
                statLabels[_StatConversion[(PrimaryStat)i]].text = ((int)stats[i]).ToString();
        }
    }

    private static float[] GetAccumulatedStats(List<float[]> stats)
    {
        float[] accumulatedStats = new float[stats[0].Length];
        for (int i = 0; i < (int)PrimaryStat.SLASH_RES; i++)
        {
            foreach (float[] stat in stats)
            {
                accumulatedStats[i] += stat[i];
            }
        }

        return accumulatedStats;
    }

    private float GetHightestStat(float[] stats)
    {
        float hightestStat = 0;
        for (int i = 0; i < _StatConversion.Count; i++)
        {    
            if (stats[i] > hightestStat)
                hightestStat = stats[i];
        }

        return hightestStat;
    }
    #endregion

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (mySpecimen!=null)
            mySpecimen.StatsChangedEvent -= DisplayAttributes;
    }
}
