using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Resistance
{
    //Resistance is displayed in a bar graph.
    public Text value;
    public Image bar;
}

public class ResistanceDisplay : MonoBehaviour
{
    //Resistances to types of damage
    [SerializeField]
    private Resistance[] _Resistances = new Resistance[System.Enum.GetValues(typeof(DamageType)).Length];

    public void DisplayResistances(Specimen specimen)
    {
        DisplayResistances(specimen.species);
    }

    public void DisplayResistances(Species species)
    {
        float hightestResistance = 25;
        for (int i = 0; i < _Resistances.Length; i++)
            if (species.baseStats[(int)PrimaryStat.SLASH_RES + i] > hightestResistance)
                hightestResistance = species.baseStats[(int)PrimaryStat.SLASH_RES + i];

        for (int i = 0; i < _Resistances.Length; i++)
        {
            if (_Resistances[i].value != null)
                _Resistances[i].value.text = species.baseStats[(int)PrimaryStat.SLASH_RES + i].ToString() + "\n" + ((int)(100 - Mathf.Pow(0.99f, species.baseStats[(int)PrimaryStat.SLASH_RES + i]) * 100)).ToString() + "%";

            _Resistances[i].bar.rectTransform.sizeDelta = new Vector2(_Resistances[i].bar.rectTransform.sizeDelta.x, species.baseStats[(int)PrimaryStat.SLASH_RES + i] / hightestResistance * 100f);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
