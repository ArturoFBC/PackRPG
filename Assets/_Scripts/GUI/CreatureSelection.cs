using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSelection : MonoBehaviour
{
    public RectTransform _DisplayPanel;
    public GameObject _CreatureDisplayMiniPrefab;

    public CreatureManagementPanel _MyCreatureManagementPanel;

    private void OnEnable()
    {
        // Destroy all creature icons in the panel
        foreach (Transform child in _DisplayPanel)
            Destroy(child.gameObject);

        // Create new ones
        foreach ( Specimen specimen in CreatureStorage.storedCreatures )
        {
            CreatureDisplay newCreaturoDisplay = Instantiate(_CreatureDisplayMiniPrefab, _DisplayPanel).GetComponentInChildren<CreatureDisplay>();
            newCreaturoDisplay.DisplaySpecimen(specimen);
        }
    }

    public void CreatureSwitch(Specimen specimen)
    {
        _MyCreatureManagementPanel.SwitchSpecimen(specimen);
    }
}
