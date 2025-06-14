using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    [CreateAssetMenu(menuName = "GameProgress/MilestoneData")]
    [System.Serializable]
    public class MilestoneData : ScriptableObject
    {
        public string id;
        public string description;

        [SerializeField] private List<GoalData> goalList;

        public List<GoalData> GetGoals()
        {
            return new List<GoalData>(goalList);
        }
    }
}