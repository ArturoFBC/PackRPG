using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseOverInfoDisplay : MonoBehaviour
{
    Text _LevelDisplay;
    Text _SpeciesDisplay;
    Text _TierDisplay;
    Image _WeaknessDisplay;
    public DividedHealthBar _HealthDisplay;
    BuffListDisplay _BuffDisplay;

    private void Awake()
    {
        _HealthDisplay = GetComponentInChildren<DividedHealthBar>();
        _BuffDisplay = GetComponentInChildren<BuffListDisplay>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "WeaknessDisplay")
            {
                _WeaknessDisplay = transform.GetChild(i).GetComponent<Image>();
            }
            else if (transform.GetChild(i).name == "LevelDisplay")
            {
                _LevelDisplay = transform.GetChild(i).GetComponent<Text>();
            }
            else if (transform.GetChild(i).name == "TierDisplay")
            {
                _TierDisplay = transform.GetChild(i).GetComponent<Text>();
            }
            else if (transform.GetChild(i).name == "SpeciesDisplay")
            {
                _SpeciesDisplay = transform.GetChild(i).GetComponent<Text>();
            }
            else if (transform.GetChild(i).name == "HealthBar")
            {
                _HealthDisplay = transform.GetChild(i).GetComponent<DividedHealthBar>();
            }
            else if (transform.GetChild(i).name == "BuffDisplay")
            {
                _BuffDisplay = transform.GetChild(i).GetComponent<BuffListDisplay>();
            }
        }
    }

    public void DisplayEnemyInfo( GameObject enemy )
    {
        CreatureStats creatureStats = enemy.GetComponent<CreatureStats>();
        Species enemySpecies = creatureStats.GetSpecimen().species;

        _SpeciesDisplay.text = name = enemySpecies.name.Substring(enemySpecies.name.IndexOf('_') + 1, enemySpecies.name.Length - enemySpecies.name.IndexOf('_') - 1);
        _SpeciesDisplay.gameObject.SetActive(true);

        _TierDisplay.text = creatureStats._MyTier.ToString();
        _TierDisplay.gameObject.SetActive(true);
        _TierDisplay.color = EnemyTierValues.color[(int)creatureStats._MyTier];
        if (_TierDisplay.color == Color.black)
            _TierDisplay.color = Color.white;
        else
            _TierDisplay.color = _TierDisplay.color * 1.3f;

        int level = enemy.GetComponent<Level>().level;
        _LevelDisplay.text = level.ToString();
        _LevelDisplay.gameObject.SetActive(true);

        _HealthDisplay.gameObject.SetActive(true);
    }

    public void DisplayName ( string name )
    {
        HideEverything();
        _SpeciesDisplay.text = name;
        _SpeciesDisplay.gameObject.SetActive(true);
    }

    public void StopDisplay()
    {
        HideEverything();
    }

    private void HideEverything()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
