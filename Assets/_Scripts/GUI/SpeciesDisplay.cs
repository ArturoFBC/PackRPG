using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesDisplay : MonoBehaviour
{
    [SerializeField] private Text  nameLabel;
    [SerializeField] private Text  numberLabel;
    [SerializeField] private Image creatureMiniature;

    [SerializeField] private SpeciesTooltipDisplayer myTooltip;

    [SerializeField] private Species mySpecies;

    public void Set(Species species, bool hideUndiscovered = false)
    {
        mySpecies = species;

        nameLabel.text = species.speciesName;
        numberLabel.text = "#" + species.speciesNumber;
        creatureMiniature.sprite = species.closeUpImage;

        if (hideUndiscovered && PackpediaManager.IsSpeciesOwned(species) == false)
        {
            creatureMiniature.color = Color.black;
            myTooltip.enabled = false;
        }
        else
        {
            creatureMiniature.color = Color.white;
            myTooltip.SetObjectToDisplay(mySpecies);
            myTooltip.enabled = true;
        }
    }

    public void Unset()
    {
        mySpecies = null;

        nameLabel.text = "";
        numberLabel.text = "";
        creatureMiniature.sprite = null;
    }
}
