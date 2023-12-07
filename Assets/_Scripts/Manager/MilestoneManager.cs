using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilestoneManager : MonoBehaviour
{
    [SerializeField] private List<MilestoneData> milestones;
    private int currentMilestone;

    private List<MilestoneGoalBase> currentGoals;

    private void ActivateMilestone(MilestoneData newMilestone)
    {
        currentGoals.Clear();

        foreach (MilestoneGoalData goalData in newMilestone.goals)
        {
            MilestoneGoalBase newGoal = MilestoneGoalBase.CreateMilestone(goalData);
            currentGoals.Add( newGoal );
            newGoal.GoalAchievedEvent += CheckMilestoneCompletedness;
        }
    }

    private void CheckMilestoneCompletedness()
    {
        bool allCompleted = true;
        foreach (MilestoneGoalBase goal in currentGoals)
        {
            if (goal.CheckGoal() == false)
            {
                allCompleted = false;
                break;
            }
        }

        if (allCompleted)
            CompleteMilestone(milestones[currentMilestone]);
    }

    private void CompleteMilestone(MilestoneData completedMilestone)
    {

    }
}
