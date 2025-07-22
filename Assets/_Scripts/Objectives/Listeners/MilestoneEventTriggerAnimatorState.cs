using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public class MilestoneEventTriggerAnimatorState : MilestoneEventListenerBase
    {
        [SerializeField] private Animator myAnimator;
        [SerializeField] private string triggerName;

        protected override void OnConditionMet()
        {
            myAnimator.SetTrigger(triggerName);
        }

    }
}