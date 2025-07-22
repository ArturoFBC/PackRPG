using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public abstract class MilestoneEventListenerBase : MonoBehaviour
    {
        [Header("Milestone")]
        [SerializeField] MilestoneData myMilestone;
        [SerializeField] MilestoneStatus milestoneStatusTrigger;



        private void OnEnable()
        {
            Milestone.MilestoneStatusUpdatedEvent += OnMilestoneUpdated;
        }

        private void OnDisable()
        {
            Milestone.MilestoneStatusUpdatedEvent -= OnMilestoneUpdated;
        }

        private void OnMilestoneUpdated(MilestoneData milestoneData, MilestoneStatus status)
        {
            if (milestoneData == myMilestone && milestoneStatusTrigger == status)
            {
                OnConditionMet();
            }
        }

        protected abstract void OnConditionMet();
    }
}
