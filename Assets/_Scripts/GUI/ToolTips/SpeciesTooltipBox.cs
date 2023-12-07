using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesTooltipBox : MonoBehaviour
{
    [SerializeField] private ResistanceDisplay resistancesDisplay;
    [SerializeField] private AttributeDisplay attributesDisplay;
    [SerializeField] private Image imageDisplay;
    [SerializeField] private Text indexDisplay;
    [SerializeField] private Text nameDisplay;
    [SerializeField] private Text descriptionDisplay;
    
    public void SetSpecies(Species species)
    {
        if ( resistancesDisplay != null )
            resistancesDisplay.DisplayResistances(species);

        if (attributesDisplay != null)
            attributesDisplay.DisplayAttributes(species);

        if (nameDisplay != null )
            nameDisplay.text = species.name;

        if (imageDisplay != null)
            imageDisplay.sprite = species.miniature;

        if (indexDisplay != null)
            indexDisplay.text = "#" + ScriptableReferencesHolder.GetSpeciesIndex(species).ToString();

        if (descriptionDisplay != null)
            descriptionDisplay.text = species.description;
    }

}
