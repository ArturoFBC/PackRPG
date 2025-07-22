using GameProgress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MilestoneEventAction : MilestoneEventListenerBase
{
    [Header("Event")]
    [SerializeField] private UnityEvent onMilestone;

    protected override void OnConditionMet()
    {
        onMilestone.Invoke();
    }
}
