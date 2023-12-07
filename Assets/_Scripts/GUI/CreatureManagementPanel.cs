using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CreatureManagementPanel : MonoBehaviour
{
    public CreatureDisplay[] _CreatureDisplays = new CreatureDisplay[GameManager.MAX_PACK_SIZE];
    public Transform _MorphPosition;
    public PlayableDirector _MorphDirector;
    public Camera _MorphCamera;
    public Camera _RenderTextureCamera;
    public LayerMask _MorphBackgroundLayer;
    public DissolveParticlesManager _ParticlesManager;
    public GameObject _SecondaryParticlesHolder;
    private int _MorphIndex;


    public CreatureSelection _MyCreatureSelectionPanel;

    private int _SpecimenToBeSwitchedIndex;

    private void OnEnable()
    {
        DisplayCurrentPack();
    }

    private void DisplayCurrentPack()
    {
        for (int activeCreatureIndex = 0; activeCreatureIndex < GameManager.MAX_PACK_SIZE; activeCreatureIndex++)
        {
            if (CreatureStorage.activePack.Count <= activeCreatureIndex)
                _CreatureDisplays[activeCreatureIndex].DisplaySpecimen( null );
            else
                _CreatureDisplays[activeCreatureIndex].DisplaySpecimen( CreatureStorage.activePack[activeCreatureIndex] );
        }
    }

    #region SWITCH SPECIMEN
    public void InitiateSwitch(int index)
    {
        _SpecimenToBeSwitchedIndex = index;
        _MyCreatureSelectionPanel.gameObject.SetActive( true );
    }

    public void SwitchSpecimen( Specimen specimen )
    {
        _MyCreatureSelectionPanel.gameObject.SetActive( false );

        int newPackMemberIndex = -1;
        for (int specimenIndex = 0; specimenIndex < CreatureStorage.storedCreatures.Count; specimenIndex++)
        {
            if ( specimen == CreatureStorage.storedCreatures[specimenIndex] )
            {
                newPackMemberIndex = specimenIndex;
                break;
            }
        }

        if (CreatureStorage.activePack.Count <= _SpecimenToBeSwitchedIndex)
        {
            CreatureStorage.activePack.Add(CreatureStorage.storedCreatures[newPackMemberIndex]);
            CreatureStorage.storedCreatures.RemoveAt(newPackMemberIndex);
        }
        else
        {
            CreatureStorage.storedCreatures[newPackMemberIndex] = CreatureStorage.activePack[_SpecimenToBeSwitchedIndex];
            CreatureStorage.activePack[_SpecimenToBeSwitchedIndex] = specimen;
        }

        DisplayCurrentPack();
    }
    #endregion

    #region MORPH SPECIMEN
    public void InitiateMorph( int index )
    {
        _MorphIndex = index;

        NoPointerTransform newTransform = new NoPointerTransform();
        newTransform.position = _MorphPosition.position;
        newTransform.scale = _MorphPosition.localScale;
        TransformChange.StartTransformChange(_CreatureDisplays[index].GetModelSpawnPoint().gameObject, newTransform, 2f, 0f, true, true, _CreatureDisplays[index].GetModelSpawnPoint().parent, true);

        // Set camera layers
        int morphLayerIndex = _CreatureDisplays[index].GetModelSpawnPoint().gameObject.layer;
        _MorphCamera.cullingMask = _MorphBackgroundLayer | (1 << morphLayerIndex);
        _RenderTextureCamera.cullingMask = _MorphCamera.cullingMask;

        // Instantiate models and apply the morph
        _ParticlesManager._Targets = _CreatureDisplays[index].GetModelSpawnPoint().gameObject;
        List<MorphStatus> morphStatuses = CreatureMorph.CheckForAvailableMorph(_CreatureDisplays[index].GetSpecimen());
        if ( morphStatuses.Count > 0 && morphStatuses[0].creatureReady )
        {
            GameObject newCreatureModelPrefab = morphStatuses[0].morph.targetSpecies.model;
            GameObject newCreatureModel = Instantiate(newCreatureModelPrefab, _CreatureDisplays[index].GetModelSpawnPoint());
            CreatureDisplay.SetLayerRecursively(newCreatureModel, morphLayerIndex);
            newCreatureModel.SetActive(false);

            CreatureStorage.activePack[index].ApplyMorph( morphStatuses[0].morph );
        }

        // Restart particle systems
        List<ParticleSystem> secondaryParticleSystems = new List<ParticleSystem>( _SecondaryParticlesHolder.GetComponentsInChildren<ParticleSystem>() );
        foreach (ParticleSystem ps in secondaryParticleSystems)
        {
            ps.Clear();
            ps.Simulate(0.0f, true, true);
        }

        _MorphDirector.time = _MorphDirector.initialTime;
        _MorphDirector.Play();

        Invoke("FinishMorph", 14f);
    }

    void FinishMorph()
    {
        NoPointerTransform newTransform = new NoPointerTransform();
        newTransform.position = Vector3.zero;
        newTransform.scale = _CreatureDisplays[_MorphIndex].GetModelSpawnPoint().parent.localScale;
        TransformChange.StartTransformChange(_CreatureDisplays[_MorphIndex].GetModelSpawnPoint().gameObject, newTransform, 2f, 0f, true, false, _CreatureDisplays[_MorphIndex].GetModelSpawnPoint().parent, true);

        _CreatureDisplays[_MorphIndex].DisplaySpecimen(CreatureStorage.activePack[_MorphIndex]);
    }
    #endregion

    public void MoveCreatureFromActivePackToStorage(int removedCreatureIndex)
    {
        CreatureStorage.MoveCreatureFromActivePackToStorage(removedCreatureIndex);
        DisplayCurrentPack();
    }
}
