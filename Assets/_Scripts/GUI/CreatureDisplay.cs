using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureDisplay : MonoBehaviour
{
    Specimen _MySpecimen;

    //Model spawn point
    [SerializeField] private Transform _ModelSpawnPoint;

    //General creature info
    [SerializeField] private Text _Name;
    [SerializeField] private Text _SpeciesNumberAndName;
    [SerializeField] private Text _SpeciesDescription;

    //Level and experience
    [SerializeField] private Text _LevelDisplay;
    [SerializeField] private Text _ExperienceDisplay;
    [SerializeField] private Image _ExperienceBar;

    //Skills
    [SerializeField] private SkillDisplay _BasicSkill;
    [SerializeField] private SkillDisplay _CooldownSkill_1;
    [SerializeField] private SkillDisplay _CooldownSkill_2;
    [SerializeField] private SkillDisplay _Ultimate;
    [SerializeField] private SkillDisplay _Passive_1;
    [SerializeField] private SkillDisplay _Passive_2;
    [SerializeField] private SkillDisplay _Passive_3;

    //Items
    [SerializeField] private EquipmentDisplay _EquipmentDisplay;

    //Stats
    [SerializeField] private AttributeDisplay _AttributeDisplay;
    //Resistances to types of damage
    [SerializeField] private ResistanceDisplay _Resistances;

    //Morph available button
    [SerializeField] private GameObject _MorphButton;



    //Display specimen from specimen saved data
    public void DisplaySpecimen( Specimen specimen )
    {
        _MySpecimen = specimen;

        if (specimen == null)
        {
            GoBlank();
            return;
        }

        #region MODEL
        if ( _ModelSpawnPoint != null )
        {
            if (specimen.species.model != null)
            {
                // Delete everything but the correct model, OR, if the correct model is not there, delete everything and create it.
                bool alreadyExists = false;

                foreach (Transform child in _ModelSpawnPoint)
                {
                    if (child.name == specimen.species.model.name && alreadyExists == false)
                    {
                        alreadyExists = true;
                        continue;
                    }
                    Destroy(child.gameObject);
                }

                if (alreadyExists == false)
                {
                    GameObject newModel = Instantiate(specimen.species.model, _ModelSpawnPoint);
                    // Update layer to that of the parent object so only the camera with the proper layermask paints it
                    SetLayerRecursively(newModel, _ModelSpawnPoint.gameObject.layer);
                }
            }
        }
        #endregion

        if ( _Name != null )
            _Name.text = specimen.name;
        if ( _SpeciesNumberAndName != null )
            _SpeciesNumberAndName.text = specimen.species.name;
        if (_SpeciesDescription != null)
            _SpeciesDescription.text = specimen.species.description;

        #region EXPERIENCE
        if (_LevelDisplay != null || _ExperienceBar != null || _ExperienceDisplay != null)
        {
            int level = Level.CalculateLevel(specimen.exp);
            float levelExpDiference = Level.ExpLevels[level + 1] - Level.ExpLevels[level];
            float currentProgress = specimen.exp - Level.ExpLevels[level];

            if ( _LevelDisplay != null )
                _LevelDisplay.text = "Lv " + Level.CalculateLevel(specimen.exp);
            if (_ExperienceBar != null )
                _ExperienceBar.fillAmount = currentProgress / levelExpDiference;
            if (_ExperienceDisplay != null )
                _ExperienceDisplay.text = ((int)(levelExpDiference - currentProgress)).ToString() + " exp to level " + (level + 1).ToString();
        }
        #endregion

        #region SKILLS
        if ( _BasicSkill != null )
            _BasicSkill.Set(specimen.basicAttack);

        if (_CooldownSkill_1 != null )
            _CooldownSkill_1.Set(specimen.cooldownAttack_1);

        if (_CooldownSkill_2 != null )
            _CooldownSkill_2.Set(specimen.cooldownAttack_2);

        if (_Ultimate != null )
            _Ultimate.Set(specimen.ultimateAttack);

        if (_Passive_1 != null )
            _Passive_1.Set(specimen.passive_1);

        if (_Passive_2 != null )
            _Passive_2.Set(specimen.passive_2);

        if (_Passive_3 != null )
            _Passive_3.Set(specimen.passive_3);
        #endregion

        //Stats
        if ( _AttributeDisplay != null )
            _AttributeDisplay.DisplayAttributes(specimen);

        //Resistances
        if ( _Resistances != null )
            _Resistances.DisplayResistances(specimen);

        //Equipment
        if (_EquipmentDisplay != null)
            _EquipmentDisplay.Display(specimen.equipmentSlots);

        //Morph button
        if ( _MorphButton != null )
        {
            List<MorphStatus> possibleMorphs = CreatureMorph.CheckForAvailableMorph(specimen);

            _MorphButton.SetActive(false);
            foreach ( MorphStatus morphStatus in possibleMorphs)
            {
                if (morphStatus.creatureReady)
                {
                    _MorphButton.SetActive(true);
                    break;
                }
            }
        }
    }

    //Display specimen from ingame gameobject
    public void DisplayGameObject( GameObject creatureGO )
    {
        CreatureStats creatureStats = creatureGO.GetComponent<CreatureStats>();
        _MySpecimen = creatureStats.GetSpecimen();

        //_Name.text = specimen.name;
        _SpeciesNumberAndName.text = _MySpecimen.species.name;

        #region EXPERIENCE
        if (_LevelDisplay != null || _ExperienceBar != null || _ExperienceDisplay != null)
        {
            Level creatureLevel = creatureGO.GetComponent<Level>();
            int level = creatureLevel.level;
            float levelExpDiference = Level.ExpLevels[level + 1] - Level.ExpLevels[level];
            float currentProgress = creatureLevel.exp - Level.ExpLevels[level];

            if (_LevelDisplay != null)
                _LevelDisplay.text = "Lv " + Level.CalculateLevel(creatureLevel.exp);
            if (_ExperienceBar != null)
                _ExperienceBar.fillAmount = currentProgress / levelExpDiference;
            if (_ExperienceDisplay != null)
                _ExperienceDisplay.text = ((int)(levelExpDiference - currentProgress)).ToString() + " exp to level " + (level + 1).ToString();
        }
        #endregion

        #region skills
        if (_BasicSkill != null)
            _BasicSkill.Set(_MySpecimen.basicAttack);

        if (_CooldownSkill_1 != null)
            _CooldownSkill_1.Set(_MySpecimen.cooldownAttack_1);

        if (_CooldownSkill_2 != null)
            _CooldownSkill_2.Set(_MySpecimen.cooldownAttack_2);

        if (_Ultimate != null)
            _Ultimate.Set(_MySpecimen.ultimateAttack);

        if (_Passive_1 != null)
            _Passive_1.Set(_MySpecimen.passive_1);

        if (_Passive_2 != null)
            _Passive_2.Set(_MySpecimen.passive_2);

        if (_Passive_3 != null)
            _Passive_3.Set(_MySpecimen.passive_3);
        #endregion

        //Stats
        if (_AttributeDisplay != null)
            _AttributeDisplay.DisplayAttributes(_MySpecimen);

        //Resistances
        if (_Resistances != null)
            _Resistances.DisplayResistances(_MySpecimen);

        //Equipment
        if (_EquipmentDisplay != null)
            _EquipmentDisplay.Display(_MySpecimen.equipmentSlots);
    }

    public Specimen GetSpecimen()
    {
        return _MySpecimen;
    }

    public Transform GetModelSpawnPoint()
    {
        return _ModelSpawnPoint;
    }

    #region TOOLS
    public static void SetLayerRecursively(GameObject go, int layerNumber)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }
    #endregion

    private void GoBlank()
    {
        if (_ModelSpawnPoint != null)
            foreach (Transform child in _ModelSpawnPoint)
                Destroy(child.gameObject);

        if (_Name != null)
            _Name.text = "";
        if (_SpeciesNumberAndName != null)
            _SpeciesNumberAndName.text = "";
        if (_LevelDisplay != null)
            _LevelDisplay.text = "";
        if (_ExperienceDisplay != null)
            _ExperienceDisplay.text = "";
        if (_ExperienceBar != null)
            _ExperienceBar.fillAmount = 0;


        if (_BasicSkill != null) _BasicSkill.Set(null);
        if (_CooldownSkill_1 != null) _CooldownSkill_1.Set(null);
        if (_CooldownSkill_2 != null) _CooldownSkill_2.Set(null);
        if (_Ultimate != null) _Ultimate.Set(null);
        if (_Passive_1 != null) _Passive_1.Set(null);
        if (_Passive_2 != null) _Passive_2.Set(null);
        if (_Passive_3 != null) _Passive_3.Set(null);

        if (_EquipmentDisplay != null)
            _EquipmentDisplay.Clear();

        _AttributeDisplay.Hide();
        _Resistances.Hide();

        _MorphButton.SetActive(false);
    }
}
