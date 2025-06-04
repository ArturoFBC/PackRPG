using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public class Mission
    {
        private MissionData myMissionData;

        private List<Milestone> milestones;
        private int currentMilestoneIndex;

        public Mission(MissionData missionData, Dictionary<string,Dictionary<string,int>> missionCompletion)
        {
            myMissionData = missionData;

            milestones = new List<Milestone>();
            foreach (MilestoneData milestoneData in missionData.GetMilestones())
            {
                Dictionary<string, int> milestoneCompletion = new Dictionary<string, int>();
                if (missionCompletion.ContainsKey(myMissionData.id))
                {
                    milestoneCompletion = missionCompletion[myMissionData.id];
                }

                milestones.Add(new Milestone(milestoneData, milestoneCompletion));
            }

            for(int i = 0; i < milestones.Count; i++)
            {
                if (milestones[i].IsCompleted() == false)
                {
                    currentMilestoneIndex = i;
                    break;
                }
            }
        }

        public Milestone GetCurrentMilestone()
        {
            return milestones[currentMilestoneIndex];
        }

        public bool IsCompleted()
        {
            return currentMilestoneIndex < 0;
        }

        internal string GetId()
        {
            return myMissionData.id;
        }

        internal IEnumerable<Milestone> GetMilestones()
        {
            return milestones;
        }
    }
}