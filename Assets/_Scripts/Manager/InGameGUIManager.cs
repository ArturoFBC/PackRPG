using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameGUIManager : MonoBehaviour
{
    public static InGameGUIManager _Ref;
    public MouseOverInfoDisplay _MouseOverInfoDisplay;

    public List<DividedHealthBar> _PlayersHealthBars = new List<DividedHealthBar>();
    public List<DividedHealthBar> _PlayersManaBars = new List<DividedHealthBar>();

    public List<CreatureHitPoints> _Lifes   = new List<CreatureHitPoints>();
    public List<CreatureMana> _Manas        = new List<CreatureMana>();
    public List<Level> _Levels              = new List<Level>();
    public List<CreatureStats> _Stats       = new List<CreatureStats>();
    public List<GameObject> _CreatureHuds   = new List<GameObject>();
 
    // Sub-menu panels
    [SerializeField] private GameObject _CreaturePanel;
    [SerializeField] private GameObject _SkillLearnPanel;
    [SerializeField] private GameObject _InventoryPanel;
    [SerializeField] private GameObject _PauseMenuPanel;
    [SerializeField] private GameObject _AreaExitPanel;
    [SerializeField] private GameObject _DefeatPanel;
    [SerializeField] private GameObject _PackPediaPanel;

    private void Awake()
    {
        if (_Ref == null)
            _Ref = this;
        else
            DestroyImmediate(this);

        GameManager.playerCreatureAddedEvent += OnPlayerCreatureAdded;
        GetPanelReferences();
        HideUnusedCreatureHuds();
    }

    private void GetPanelReferences()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "CreaturePanel")
            {
                _CreaturePanel = transform.GetChild(i).gameObject;
            }
            else if (transform.GetChild(i).name == "SkillLearnPanel")
            {
                _SkillLearnPanel = transform.GetChild(i).gameObject;
            }
            else if (transform.GetChild(i).name == "InventoryPanel")
            {
                _InventoryPanel = transform.GetChild(i).gameObject;
            }
            else if (transform.GetChild(i).name == "PauseMenuPanel")
            {
                _PauseMenuPanel = transform.GetChild(i).gameObject;
            }
            else if (transform.GetChild(i).name == "AreaExitPanel")
            {
                _AreaExitPanel = transform.GetChild(i).gameObject;
            }
            else if (transform.GetChild(i).name == "MouseOverInfoDisplay")
            {
                _MouseOverInfoDisplay = transform.GetChild(i).GetComponent<MouseOverInfoDisplay>();
            }
            else if (transform.GetChild(i).name == "DefeatPanel")
            {
                _DefeatPanel = transform.GetChild(i).gameObject;
            }
            else if (transform.GetChild(i).name == "PackpediaPanel")
            {
                _PackPediaPanel = transform.GetChild(i).gameObject;
            }
        }

        for (int i = 0; i < GameManager.MAX_PACK_SIZE; i++)
        {
            GameObject hudCreatureGO = GameObject.Find("HudCreature" + i.ToString());
            if (hudCreatureGO == null)
            {
                Debug.LogError("Can't find hud of creature " + i.ToString() + " to configure");
                break;
            }
            else
            {
                _CreatureHuds.Add(hudCreatureGO);
            }
        }
    }

    private void Start()
    {
        for (int playerCreatureIndex = 0; playerCreatureIndex < GameManager.playerCreatures.Count; playerCreatureIndex++)
            OnPlayerCreatureAdded(playerCreatureIndex);
    }

    private void OnDestroy()
    {
        GameManager.playerCreatureAddedEvent -= OnPlayerCreatureAdded;
    }

    public void OnPlayerCreatureAdded( int newPlayerCreatureIndex )
    {
        _CreatureHuds[newPlayerCreatureIndex].SetActive(true);
        GetPlayerReferencesToBeLinked(newPlayerCreatureIndex);
        LinkHudComponentsToCreature(newPlayerCreatureIndex);
    }

    private void LinkHudComponentsToCreature(int playerCreatureIndex)
    {
        LinkHealthBar(playerCreatureIndex);
        LinkConscienceBar(playerCreatureIndex);
        LinkManaBar(playerCreatureIndex);
        LinkExperienceBar(playerCreatureIndex);
        LinkLevelDisplay(playerCreatureIndex);
        LinkModifierDisplay(playerCreatureIndex);
        LinkButtonToLearnNewSkills(playerCreatureIndex);
    }

    #region LINK_HUD_TO_CREATURE
    private void LinkButtonToLearnNewSkills(int playerCreatureIndex)
    {
        LearnSkillButton learnSkillButton = _CreatureHuds[playerCreatureIndex].GetComponentInChildren<LearnSkillButton>(true);
        if (learnSkillButton != null)
        {
            learnSkillButton.Set(_Stats[playerCreatureIndex]);
            _Levels[playerCreatureIndex].levelUpEvent += learnSkillButton.CheckForSkills;
            GameManager.playerCreatures[playerCreatureIndex].GetComponent<CreatureSkillManager>().SkillsChangedEvent += learnSkillButton.CheckForSkills;
        }

        HudCreature hudCreature = _CreatureHuds[playerCreatureIndex].GetComponent<HudCreature>();
        hudCreature.Set(GameManager.playerCreatures[playerCreatureIndex]);
    }

    private void LinkModifierDisplay(int playerCreatureIndex)
    {
        GameObject buffDisplayGo = _CreatureHuds[playerCreatureIndex].transform.Find("BuffDisplay").gameObject;
        BuffListDisplay buffListDisplay;
        if (buffDisplayGo != null)
        {
            buffListDisplay = buffDisplayGo.GetComponent<BuffListDisplay>();
            if (buffListDisplay != null)
            {
                _Stats[playerCreatureIndex].AddedBuffEvent += buffListDisplay.AddBuffIcon;
                _Stats[playerCreatureIndex].RemovedBuffEvent += buffListDisplay.RemoveBuffIcon;
            }
        }
    }

    private void LinkLevelDisplay(int playerCreatureIndex)
    {
        GameObject levelDisplayGo = _CreatureHuds[playerCreatureIndex].transform.Find("LevelDisplay").gameObject;
        LevelDisplay levelDisplay;
        if (levelDisplayGo != null)
        {
            levelDisplay = levelDisplayGo.GetComponent<LevelDisplay>();
            if (levelDisplay != null)
            {
                levelDisplay.UpdateLevel(_Levels[playerCreatureIndex].level);
                _Levels[playerCreatureIndex].levelUpEvent += levelDisplay.UpdateLevel;
            }
        }
    }

    private void LinkExperienceBar(int playerCreatureIndex)
    {
        GameObject expBarGO = _CreatureHuds[playerCreatureIndex].transform.Find("ExpBar").gameObject;
        DividedHealthBar expBar;
        if (expBarGO != null)
        {
            expBar = expBarGO.GetComponent<DividedHealthBar>();
            if (expBar != null)
            {
                float maxExp = Level.ExpLevels[_Levels[playerCreatureIndex].level + 1] - Level.ExpLevels[_Levels[playerCreatureIndex].level];
                expBar.SetMaxValue(maxExp);
                expBar.SetValue(_Levels[playerCreatureIndex].exp - Level.ExpLevels[_Levels[playerCreatureIndex].level], maxExp);
                _Levels[playerCreatureIndex].maxExpChangedEvent += expBar.SetMaxValue;
                _Levels[playerCreatureIndex].expChangedEvent += expBar.SetValue;
            }
        }
    }

    private void LinkManaBar(int playerCreatureIndex)
    {
        GameObject manaBarGO = _CreatureHuds[playerCreatureIndex].transform.Find("ManaBar").gameObject;
        DividedHealthBar manaBar;
        if (manaBarGO != null)
        {
            manaBar = manaBarGO.GetComponent<DividedHealthBar>();
            if (manaBar != null)
            {
                manaBar.SetMaxValue(_Manas[playerCreatureIndex].MaxMP);
                manaBar.SetValue(_Manas[playerCreatureIndex].currentMP, _Manas[playerCreatureIndex].MaxMP);
                _Manas[playerCreatureIndex].maxManaChangedEvent += manaBar.SetMaxValue;
                _Manas[playerCreatureIndex].manaChangedEvent += manaBar.SetValue;
            }
        }
    }

    private void LinkConscienceBar(int playerCreatureIndex)
    {
        GameObject conscienceBarGO = _CreatureHuds[playerCreatureIndex].transform.Find("ConscienceBar").gameObject;
        DividedHealthBar conscienceBar;
        if (conscienceBarGO != null)
        {
            conscienceBar = conscienceBarGO.GetComponent<DividedHealthBar>();
            if (conscienceBar != null)
            {
                conscienceBar.SetMaxValue(_Lifes[playerCreatureIndex].MaxHP);
                conscienceBar.SetValue(_Lifes[playerCreatureIndex].consciousness, _Lifes[playerCreatureIndex].MaxHP);
                _Lifes[playerCreatureIndex].MaxHealthChangedEvent += conscienceBar.SetMaxValue;
                _Lifes[playerCreatureIndex].ConsciousnessChangedEvent += conscienceBar.SetValue;
            }
        }
    }

    private void LinkHealthBar(int playerCreatureIndex)
    {
        GameObject healthBarGO = _CreatureHuds[playerCreatureIndex].transform.Find("HealthBar").gameObject;
        DividedHealthBar healthBar;
        if (healthBarGO != null)
        {
            healthBar = healthBarGO.GetComponent<DividedHealthBar>();
            if (healthBar != null)
            {
                healthBar.SetMaxValue(_Lifes[playerCreatureIndex].MaxHP);
                healthBar.SetValue(_Lifes[playerCreatureIndex].currentHP, _Lifes[playerCreatureIndex].MaxHP);
                _Lifes[playerCreatureIndex].MaxHealthChangedEvent += healthBar.SetMaxValue;
                _Lifes[playerCreatureIndex].HealthChangedEvent += healthBar.SetValue;
            }
        }
    }

    private void GetPlayerReferencesToBeLinked(int i)
    {
        //Get life and mana
        if (_Lifes.Count <= i)
            _Lifes.Add(GameManager.playerCreatures[i].GetComponent<CreatureHitPoints>());
        else 
            _Lifes[i] = GameManager.playerCreatures[i].GetComponent<CreatureHitPoints>();

        if (_Manas.Count <= i)
            _Manas.Add(GameManager.playerCreatures[i].GetComponent<CreatureMana>());
        else
            _Manas[i] = GameManager.playerCreatures[i].GetComponent<CreatureMana>();

        if (_Levels.Count <= i)
            _Levels.Add(GameManager.playerCreatures[i].GetComponent<Level>());
        else
            _Levels[i] = GameManager.playerCreatures[i].GetComponent<Level>();

        if (_Stats.Count <= i)
            _Stats.Add(GameManager.playerCreatures[i].GetComponent<CreatureStats>());
        else
            _Stats[i] = GameManager.playerCreatures[i].GetComponent<CreatureStats>();
    }

    private static void HideUnusedCreatureHuds()
    {
        for (int i = GameManager.playerCreatures.Count; i < GameManager.MAX_PACK_SIZE; i++)
        {
            GameObject hudCreatureGO = GameObject.Find("HudCreature" + i.ToString());
            if (hudCreatureGO != null)
                hudCreatureGO.SetActive(false);
        }
    }
    #endregion

    #region PANEL_MANAGEMENT
    public void ShowEnemyInfo(GameObject enemy)
    {
        if (enemy != null)
            _MouseOverInfoDisplay.DisplayEnemyInfo(enemy);
        else
            _MouseOverInfoDisplay.StopDisplay();
    }

    public void ShowInteractableInfo(GameObject interactable)
    {
        if (interactable != null)
            _MouseOverInfoDisplay.DisplayName(interactable.name);
        else
            _MouseOverInfoDisplay.StopDisplay();
    }

    public void OpenCreaturePanel()
    {
        _CreaturePanel.SetActive(true);
        GlobalSounds._Ref.Play(SoundID.OpenMenu);
    }

    public void OpenInventoryPenal()
    {
        _InventoryPanel.SetActive(true);
        GlobalSounds._Ref.Play(SoundID.OpenMenu);
    }

    public void OpenSkillLearnPanel(CreatureSkillManager skillManagerToNotify, BaseSkill newSkill, List<BaseSkill> knownSkills )
    {
        if (_SkillLearnPanel != null)
            _SkillLearnPanel.GetComponent<SkillLearnPanel>().FillPanel(skillManagerToNotify, newSkill, knownSkills);
    }

    public void OpenPauseMenuPanel()
    {
        if (_PauseMenuPanel != null)
            _PauseMenuPanel.SetActive(true);
    }

    public void ClosePauseMenuPanel()
    {
        if (_PauseMenuPanel != null)
            _PauseMenuPanel.SetActive(false);
    }

    public void OpenAreaExitPanel( Area nextArea )
    {
        _AreaExitPanel.GetComponentInChildren<AreaExitPanel>().ShowPanel(nextArea);
    }

    public void CloseAreaExitPanel()
    {
        _AreaExitPanel.SetActive(false);
    }

    public void OpenDefeatPanel()
    {
        _DefeatPanel.SetActive(true);
    }

    public void ClosePackPediaPanel()
    {
        _PackPediaPanel.SetActive(false);
    }

    public void OpenPackPediaPanel()
    {
        _PackPediaPanel.SetActive(true);
    }
    #endregion
}
