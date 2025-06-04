using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameProgress
{
    public class Milestone
    {
        private MilestoneData myData;

        private Dictionary<GoalData, int> goalProgress;

        public Action<Milestone, GoalData, int> GoalPogressEvent;

        public Milestone(MilestoneData myData, Dictionary<string, int> initialGoalProgress )
        {
            this.myData = myData;

            goalProgress = new Dictionary<GoalData, int>();
            foreach (GoalData goal in myData.GetGoals())
            {
                int currentProgress = 0;

                if (initialGoalProgress.ContainsKey(goal.id))
                    currentProgress = initialGoalProgress[goal.id];

                goalProgress.Add(goal, currentProgress);
            }
        }

        public void ReportProgress(string id, int progress)
        {
            GoalData currentGoal = goalProgress.Keys.First(k => k.id == id);

            if (goalProgress.ContainsKey(currentGoal))
            {
                goalProgress[currentGoal] += progress;
                GoalPogressEvent?.Invoke(this, currentGoal, goalProgress[currentGoal]);
            }
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
