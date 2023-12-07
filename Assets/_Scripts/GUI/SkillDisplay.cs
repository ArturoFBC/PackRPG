using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDisplay : MonoBehaviour
{
    public Image _Icon;
    public Text _Name;
    public SkillTooltipDisplayer _Tooltip;

    public void Set(BaseSkill baseSkill)
    {
        if (baseSkill == null)
        {
            GoBlank();
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }


        if ( _Name != null )
            _Name.text = baseSkill.name;

        if (_Tooltip != null)
        {
            _Tooltip.SetObjectToDisplay(baseSkill);
            _Tooltip.enabled = true;
        }

        if (_Icon != null)
        {
            _Icon.enabled = true;
            _Icon.sprite = baseSkill.icon;
        }
    }

    private void GoBlank()
    {
        if (_Icon != null)
            _Icon.enabled = false;

        if (_Name != null)
            _Name.text = "";

        if (_Tooltip != null)
            _Tooltip.enabled = false;
    }
}
