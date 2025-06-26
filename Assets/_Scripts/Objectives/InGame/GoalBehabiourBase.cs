using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public abstract class GoalBehabiourBase : MonoBehaviour
    {
        [SerializeField] protected GoalData goalThisCompletes;
        [SerializeField] protected int goalAdded = 1;

        protected virtual void Awake()
        {
            SetStatus(GameProgressManager.GetMilestoneStatus(goalThisCompletes));
            Milestone.MilestoneStatusUpdatedEvent += OnMilestoneUpdated;
        }

        protected void OnMilestoneUpdated(MilestoneData milestoneData, MilestoneStatus status)
        { 
            if (milestoneData.GetGoals().Contains(goalThisCompletes))
            {
                Debug.Log(milestoneData.id + " " + status.ToString());
                SetStatus(status);
            }
        }

        protected void SetStatus(MilestoneStatus status)
        {
            Debug.Log(">>> Goal status " + status.ToString());
            switch (status)
            {
                case MilestoneStatus.NOT_STARTED:
                    SetNotStarted();
                    break;
                case MilestoneStatus.IN_PROGRESS:
                    SetInProgress();
                    break;
                case MilestoneStatus.COMPLETED:
                    SetCompleted();
                    break;
            }
        }

        protected abstract void SetNotStarted();

        protected abstract void SetInProgress();

        protected abstract void SetCompleted();

        protected void OnActivation()
        {
            GameProgressManager.ReportGoal(goalThisCompletes, goalAdded);
        }
    }
}