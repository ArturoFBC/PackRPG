using System;
using System.Collections.Generic;
using UnityEngine;
using SaveLoad;
using System.Linq;
using UnityEditorInternal.VersionControl;
using Interface.InGame.Console;

namespace GameProgress
{
    public enum MilestoneStatus
    {
        NOT_STARTED,
        IN_PROGRESS,
        COMPLETED
    }

    [Serializable]
    public class Milestone
    {
        [SerializeField] private MilestoneData myData;

        [SerializeField] private Dictionary<GoalData, int> goalProgress;

        [SerializeField] public MilestoneStatus myStatus { get; private set; }

        public static Action<MilestoneData, MilestoneStatus> MilestoneStatusUpdatedEvent;
        public Action<MilestoneData, GoalData, int> GoalPogressEvent;


        public Milestone(MilestoneData newData, MilestoneSaveData milestoneSaveData )
        {
            Construct(newData, milestoneSaveData);
        }

        public Milestone(MilestoneData newData)
        {
            Construct(newData, new MilestoneSaveData());
        }

        private void Construct(MilestoneData newData, MilestoneSaveData milestoneSaveData)
        {
            myData = newData;

            goalProgress = new Dictionary<GoalData, int>();
            foreach (GoalData goal in myData.GetGoals())
            {
                int currentProgress = 0;

                if (milestoneSaveData.Equals(default(MilestoneSaveData)) == false)
                {
                    GoalSaveData goalSaveData = milestoneSaveData.goalCurrentValues.FirstOrDefault(x => x.id == goal.id);

                    // If the data with the correct id could be found
                    if (goalSaveData.Equals(default(GoalSaveData)) == false)
                    {
                        currentProgress = goalSaveData.currentValue;
                    }
                }

                goalProgress.Add(goal, currentProgress);
            }

            ChangeStatus(MilestoneStatus.IN_PROGRESS);
            ConsolePanel.Ref.DisplayMessage($"Milestone activated < {myData.id} > ");
            Debug.Log(">>> Milestone activated <" + myData.id + ">");
        }

        public void ReportProgress(GoalData currentGoal, int progress)
        {
            if (goalProgress.ContainsKey(currentGoal) && goalProgress[currentGoal] < currentGoal.goalValue)
            {
                goalProgress[currentGoal] += progress;
                GoalPogressEvent?.Invoke(myData, currentGoal, goalProgress[currentGoal]);

                if (IsCompleted())
                {
                    Debug.Log("<<< Milestone completed <" + myData.id + ">");
                    ChangeStatus(MilestoneStatus.COMPLETED);
                }
            }
        }

        private void ChangeStatus(MilestoneStatus newStatus)
        {
            myStatus = newStatus;
            MilestoneStatusUpdatedEvent?.Invoke(myData, myStatus);
        }

        public bool IsCompleted()
        {
            foreach (KeyValuePair<GoalData, int> pair in goalProgress)
            {
                if (pair.Key.goalValue > pair.Value)
                    return false;
            }

            return true;
        }

        public MilestoneData GetData()
        {
            return myData;
        }

        public string GetID()
        {
            return myData.id;
        }

        public Dictionary<GoalData,int> GetGoalDatas()
        {
            return goalProgress;
        }
    }

}
