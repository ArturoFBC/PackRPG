using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelTooltipDisplayer : TooltipDisplayer
{
    [SerializeField] private string textReference;


    private void Start()
    {
        _ObjectToDisplay = textReference;
    }

    protected override void SendSetTooltip()
    {
        _InstantiatedTooltip.SendMessage("SetText", TextReferences.GetText(textReference));
    }
}
