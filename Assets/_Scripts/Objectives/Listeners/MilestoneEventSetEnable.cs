using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public class MilestoneEventSetEnable : MilestoneEventListenerBase
    {
        [Header("Targets")]
        [SerializeField] private List<MonoBehaviour> targets;
        [SerializeField] private bool activeStatus;

        protected override void OnConditionMet()
        {
            foreach (MonoBehaviour target in targets)
            {
                target.enabled = activeStatus;
            }
        }

    }
}