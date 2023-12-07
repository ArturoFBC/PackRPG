using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class CreaturePanel : MonoBehaviour
{
    private CreatureDisplay _MyCreatureDisplay;

    public RawImage _Avatar;

    int _SpecimenIndex = 0;

    public RenderTexture[] _AvatarTextures = new RenderTexture[4];


    private void Awake()
    {
        if (_MyCreatureDisplay == null)
            _MyCreatureDisplay = GetComponent<CreatureDisplay>();
    }

    private void OnEnable()
    {
        DisplaySpecimen(_SpecimenIndex);
    }

    //Display specimen from specimen saved data
    void DisplaySpecimen(int index)
    {
        //Ensure there is a specimen to show
        if (index >= CreatureStorage.activePack.Count || CreatureStorage.activePack[index] == null)
            return;

        Specimen specimen = CreatureStorage.activePack[index];
        _MyCreatureDisplay.DisplaySpecimen(specimen);

        _Avatar.texture = _AvatarTextures[index];
    }

    //Display specimen from ingame gameobject
    void DisplaySpecimenFromGameObject(int index)
    {
        //Ensure there is a specimen to show
        if (index >= GameManager.playerCreatures.Count || GameManager.playerCreatures[index] == null)
            return;

        GameObject creatureGO = GameManager.playerCreatures[index];
        _MyCreatureDisplay.DisplayGameObject(creatureGO);

        _Avatar.texture = _AvatarTextures[index];
    }

    public void DisplayNext()
    {
        if (_SpecimenIndex + 1 >= CreatureStorage.activePack.Count || CreatureStorage.activePack[_SpecimenIndex + 1] == null)
            _SpecimenIndex = 0;
        else
            _SpecimenIndex++;

        DisplaySpecimen(_SpecimenIndex);
    }

    public void DisplayPrevious()
    {
        if (_SpecimenIndex - 1 < 0)
            _SpecimenIndex = CreatureStorage.activePack.Count - 1;
        else
            _SpecimenIndex--;

        DisplaySpecimen(_SpecimenIndex);
    }
}
