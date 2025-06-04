using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public class MissionData : ScriptableObject
    {
        public string id { get; private set; }
        public string description { get; private set; }

        [SerializeField] private List<MilestoneData> milestones;

        public List<MilestoneData> GetMilestones()
        {
            return new List<MilestoneData>(milestones);
        }
    }
}