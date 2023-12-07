using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTooltipDisplayer : TooltipDisplayer
{
    protected override void SendSetTooltip()
    {
        if (_ObjectToDisplay is BaseSkill)
        {
            _InstantiatedTooltip.SendMessage("SetSkill", (BaseSkill)_ObjectToDisplay);
        }
    }
}
