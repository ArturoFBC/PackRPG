using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameProgress
{
    public class Goal
    {
        public string id;
        public string description;
        public int goalValue;
    }

    public class MilestoneData : ScriptableObject
    {
        public string id { get; private set; }
        public string description { get; private set; }

        [SerializeField] private List<Goal> goalList;

        public List<Goal> GetGoals()
        {
            return new List<Goal>(goalList);
        }
    }

    public class Milestone
    {
        private MilestoneData myData;

        private Dictionary<Goal, int> goalProgress;

        public Action<Milestone, Goal, int> GoalPogressEvent;

        Milestone(MilestoneData myData, Dictionary<string, int> initialGoalProgress )
        {
            this.myData = myData;

            goalProgress = new Dictionary<Goal, int>();
            foreach (Goal goal in myData.GetGoals())
            {
                int currentProgress = 0;

                if (initialGoalProgress.ContainsKey(goal.id))
                    currentProgress = initialGoalProgress[goal.id];

                goalProgress.Add(goal, currentProgress);
            }
        }

        public void ReportProgress(string id, int progress)
        {
            Goal currentGoal = goalProgress.Keys.First(k => k.id == id);

            if (goalProgress.ContainsKey(currentGoal))
            {
                goalProgress[currentGoal] += progress;
                GoalPogressEvent?.Invoke(this, currentGoal, goalProgress[currentGoal]);
            }
        }

        public bool IsCompleted()
        {
            foreach (KeyValuePair<Goal, int> pair in goalProgress)
            {
                if (pair.Key.goalValue > pair.Value)
                    return false;
            }

            return true;
        }
    }

}
