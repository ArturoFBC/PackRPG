using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillLearnPanel : MonoBehaviour
{ 
    //UI elements
    public GameObject _AcceptButton;
    public GameObject _InstructionsText;

    public GameObject _SkillDisplayPrefab;

    public SkillDisplay _NewSkillDisplay;
    public RectTransform _KnownSkillsContainer;

    private List<SkillDisplay> _KnownSkillDisplays = new List<SkillDisplay>();
    private List<Toggle> _SkillToggles = new List<Toggle>();

    //Component to notify
    private CreatureSkillManager _CreatureSkillManagerToNotify;
    private BaseSkill _SkillToDecideAbout;


    public void ToggleChanged(bool b)
    {
        int activeToggles = 0;
        foreach (Toggle t in _SkillToggles)
            if (t.isOn) activeToggles++;

        _AcceptButton.SetActive(activeToggles == _KnownSkillDisplays.Count);
        _InstructionsText.SetActive(activeToggles != _KnownSkillDisplays.Count);
    }

    public void FillPanel(CreatureSkillManager skillManagerToNotify, BaseSkill newSkill, List<BaseSkill> knownSkills )
    {
        if (skillManagerToNotify == null)
        {
            Debug.LogError("Must have a skill manager to notify skill selection");
            return;
        }

        _SkillToDecideAbout = newSkill;
        _CreatureSkillManagerToNotify = skillManagerToNotify;
        _InstructionsText.GetComponent<Text>().text = GetInstructionText( newSkill );
        _NewSkillDisplay.Set(newSkill);

        DestroyKnownSkillDisplays();
        CreateKnownSkillDisplays(knownSkills);

        gameObject.SetActive(true);
    }

    private string GetInstructionText(BaseSkill newSkill)
    {
        // LOCALIZE
        string instructionText;

        int numberOfSkillsThatMustBeSelected = 0;

        if (newSkill is BaseActiveSkill)
        {
            BaseActiveSkill newSkillAsActiveSkill = newSkill as BaseActiveSkill;
            numberOfSkillsThatMustBeSelected = BaseSkill.MAX_SKILLS[(int)newSkillAsActiveSkill.category];
        }
        else
            numberOfSkillsThatMustBeSelected = 3;

        instructionText = "Select " + numberOfSkillsThatMustBeSelected + " skills.";
        return instructionText;
    }

    private void CreateKnownSkillDisplays(List<BaseSkill> knownSkills)
    {
        for (int i = 0; i < knownSkills.Count; i++)
        {
            SkillDisplay knownSkillDisplay = Instantiate(_SkillDisplayPrefab, _KnownSkillsContainer).GetComponent<SkillDisplay>();
            _KnownSkillDisplays.Add(knownSkillDisplay);
            knownSkillDisplay.Set(knownSkills[i]);

            Toggle knownSkillDisplayToggle = knownSkillDisplay.gameObject.AddComponent<Toggle>();
            _SkillToggles.Add(knownSkillDisplayToggle);
            knownSkillDisplayToggle.onValueChanged.AddListener(ToggleChanged);
            GameObject outline = knownSkillDisplay.transform.Find("Outline").gameObject;
            outline.SetActive(true);
            knownSkillDisplayToggle.graphic = outline.GetComponent<Image>();
        }
    }

    private void DestroyKnownSkillDisplays()
    {
        foreach (SkillDisplay sd in _KnownSkillDisplays)
            Destroy(sd.gameObject);

        _KnownSkillDisplays.Clear();
        _SkillToggles.Clear();
        _SkillToggles.Add(_NewSkillDisplay.GetComponent<Toggle>());
    }

    public void NotifyDecision()
    {
        List<BaseSkill> skillsToNotify = new List<BaseSkill>();

        foreach (Toggle t in _SkillToggles)
        {
            if (t.isOn)
                skillsToNotify.Add( (BaseSkill)t.GetComponent<SkillTooltipDisplayer>()._ObjectToDisplay );
        }

        _CreatureSkillManagerToNotify.SkillLearnDecision( skillsToNotify, _SkillToDecideAbout );
    }
}
