using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    [SerializeField] private Image _FilledCooldownGauge;
    [SerializeField] private Image _Icon;
    [SerializeField] private Image _Border;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color inUseColor;

    private Skill _MySkill;

    public void Set( Skill mySkill )
    {
        OnDisable();
        _MySkill = mySkill;
        OnEnable();
    }

	void OnEnable ()
    {
        _Icon.overrideSprite = _MySkill.GetIcon();
        _MySkill.CooldownChangedEvent += UpdateFill;

        PlayerInput.skillSelectedEvent += SelectSkill;

        _MySkill.ExecutionEndedEvent += OnExecutionEnd;
        _MySkill.ExecutionStartedEvent += OnExecutionStart;
	}

    void OnDisable()
    {
        if (_MySkill != null)
        {
            _MySkill.CooldownChangedEvent -= UpdateFill;
            _MySkill.ExecutionEndedEvent -= OnExecutionEnd;
            _MySkill.ExecutionStartedEvent -= OnExecutionStart;
        }

        PlayerInput.skillSelectedEvent -= SelectSkill;
    }

    public void UpdateFill( float fill )
    {
        _FilledCooldownGauge.fillAmount = 1-fill;
    }

    public void SelectSkill( Skill selectedSkill )
    {
        if (_MySkill != null && selectedSkill == _MySkill)
        {
            _Border.enabled = true;
            _Border.color = selectedColor;
        }
        else
            _Border.enabled = false;
    }

    public void UseSkill()
    {
        if (_MySkill != null )
            PlayerInput.Ref.UseSkill(_MySkill);
    }

    public void OnExecutionStart()
    {
        _Border.enabled = true;
        _Border.color = inUseColor;
    }

    public void OnExecutionEnd()
    {
        _Border.enabled = false;
    }
}
