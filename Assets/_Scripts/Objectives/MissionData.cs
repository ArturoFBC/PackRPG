using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    [CreateAssetMenu(menuName = "GameProgress/MissionData")]
    [System.Serializable]
    public class MissionData : ScriptableObject
    {
        [SerializeField] public string id;
        [SerializeField] public string description;

        [SerializeField] private List<MilestoneData> milestones;

        public List<MilestoneData> GetMilestones()
        {
            return new List<MilestoneData>(milestones);
        }
    }
}