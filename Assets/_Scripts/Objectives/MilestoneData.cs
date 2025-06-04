using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public class MilestoneData
    {
        public string id { get; private set; }
        public string description { get; private set; }

        [SerializeField] private List<GoalData> goalList;

        public List<GoalData> GetGoals()
        {
            return new List<GoalData>(goalList);
        }
    }
}