using System;
using System.Collections.Generic;
using UnityEngine;
using SaveLoad;
using System.Linq;

namespace GameProgress
{
    [Serializable]
    public class Mission
    {
        [SerializeField] private MissionData myMissionData;

        [SerializeField] private Milestone currentMilestone;

        [SerializeField] private bool completed;

        public Mission(MissionData missionData, MissionSaveData missionCompletion)
        {
            myMissionData = missionData;

            if (missionCompletion.completed)
            {
                completed = true;
                return;
            }

            foreach (MilestoneData milestoneData in missionData.GetMilestones())
            {
                MilestoneSaveData milestoneSaveData = missionCompletion.milestoneCurrentValues.FirstOrDefault(x => x.id == milestoneData.id);

                if (milestoneSaveData.Equals(default(MilestoneSaveData)) == false
                    || milestoneSaveData.completed
                    || milestoneSaveData.goalCurrentValues == null || milestoneSaveData.goalCurrentValues.Count == 0)
                { 
                    continue;
                }

                currentMilestone = new Milestone(milestoneData, milestoneSaveData);
            }

            if (currentMilestone == null)
            {
                currentMilestone = new Milestone(missionData.GetMilestones()[0]);
            }
        }

        public Mission(MissionData missionData)
        {
            myMissionData = missionData;

            List<MilestoneData> milestoneDatas = missionData.GetMilestones();
            if ( milestoneDatas != null && milestoneDatas.Count > 0)
            {
                currentMilestone = new Milestone(milestoneDatas[0]);
                completed = false;
            }
        }

        public bool IsMilestoneCompleted(MilestoneData milestoneData)
        {
            List<MilestoneData> myMilestones = myMissionData.GetMilestones();
            int indexOfQuery = myMilestones.IndexOf(milestoneData);
            int indexOfCurrent = myMilestones.IndexOf(currentMilestone.GetData());
            if (indexOfQuery < indexOfCurrent)
                return true;

            return false;
        }

        public void ReportMilestoneGoalProgress(MilestoneData milestoneData, GoalData goalData, int addedProgress)
        {
            if (milestoneData != currentMilestone.GetData())
                return;

            currentMilestone.ReportProgress(goalData, addedProgress);

            if (currentMilestone.IsCompleted())
            {
                List<MilestoneData> myMilestones = myMissionData.GetMilestones();
                int indexOfCurrent = myMilestones.IndexOf(currentMilestone.GetData());
                if (indexOfCurrent + 1 >= myMilestones.Count)
                {
                    completed = true;
                }
                else
                {
                    currentMilestone = new Milestone(myMilestones[indexOfCurrent + 1]);
                }
                
            }
        }

        public Milestone GetCurrentMilestone()
        {
            return currentMilestone;
        }

        public bool IsCompleted()
        {
            return completed;
        }

        internal bool ContainsMilestone(MilestoneData milestone)
        {
            return myMissionData.GetMilestones().Contains(milestone);
        }

        internal string GetId()
        {
            return myMissionData.id;
        }

        internal List<MilestoneData> GetMilestones()
        {
            return myMissionData.GetMilestones();
        }

        internal IEnumerable<MilestoneData> GetCompletedMilestones()
        {
            if (completed)
                return myMissionData.GetMilestones();

            List<MilestoneData> myMilestones = myMissionData.GetMilestones();
            int indexOfCurrent = myMilestones.IndexOf(currentMilestone.GetData());

            List<MilestoneData> returnMilestones = new List<MilestoneData>();
            for (int i = 0; i < indexOfCurrent; i++)
            {
                myMilestones.Add(myMilestones[i]);
            }

            return returnMilestones;
        }

        internal MissionData GetData()
        {
            return myMissionData;
        }
    }
}