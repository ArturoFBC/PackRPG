using GameProgress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveLoad
{
    [System.Serializable]
    public struct GoalSaveData
    {
        public string id;
        public int currentValue;
        public bool completed;
    }

    [System.Serializable]
    public struct MilestoneSaveData
    {
        public string id;
        public List<GoalSaveData> goalCurrentValues;
        public bool completed;
    }

    [System.Serializable]
    public struct MissionSaveData
    {
        public string id;
        public List<MilestoneSaveData> milestoneCurrentValues;
        public bool completed;
    }

    [System.Serializable]
    public class GameProgressData
    {
        public List<MissionSaveData> missionSaveDatas = new List<MissionSaveData>();

        public GameProgressData()
        {
            foreach (Mission mission in GameProgressManager.GetMissions())
            {
                MissionSaveData missionSaveData = new MissionSaveData();
                missionSaveData.id = mission.GetId();

                missionSaveData.completed = mission.IsCompleted();
                if (missionSaveData.completed)
                {
                    missionSaveData.milestoneCurrentValues = new List<MilestoneSaveData>();
                }
                else
                {
                    missionSaveData.milestoneCurrentValues = GetMilestoneSaveData(mission);
                }

                missionSaveDatas.Add(missionSaveData);
            }
        }

        private static List<MilestoneSaveData> GetMilestoneSaveData(Mission mission)
        {
            List<MilestoneSaveData> milestonesSaveData = new List<MilestoneSaveData>();

            List<MilestoneData> milesonesData = mission.GetMilestones();
            int currentMilestoneIndex = milesonesData.IndexOf(mission.GetCurrentMilestone().GetData());

            for (int i = 0; i < milesonesData.Count; i++)
            {
                MilestoneData milestoneData = milesonesData[i];
                MilestoneSaveData milestoneSaveData = new MilestoneSaveData();
                milestoneSaveData.id = milestoneData.id;

                if (i < currentMilestoneIndex)
                {
                    milestoneSaveData.completed = true;
                    milestoneSaveData.goalCurrentValues = new List<GoalSaveData>();
                }
                else if ( i > currentMilestoneIndex)
                {
                    milestoneSaveData.completed = false;
                    milestoneSaveData.goalCurrentValues = new List<GoalSaveData>();
                }
                else
                {
                    milestoneSaveData.completed = false;
                    milestoneSaveData.goalCurrentValues = GetGoalCurrentValues(mission.GetCurrentMilestone().GetGoalDatas());
                }

                milestonesSaveData.Add(milestoneSaveData);
            }

            return milestonesSaveData;
        }

        private static List<GoalSaveData> GetGoalCurrentValues(Dictionary<GoalData, int> currentGoalValues)
        {
            List<GoalSaveData> goalsSaveData = new List<GoalSaveData>();

            foreach (KeyValuePair<GoalData, int> goalPair in currentGoalValues)
            {
                GoalSaveData goalSaveData = new GoalSaveData();
                goalSaveData.id = goalPair.Key.id;
                goalSaveData.completed = (goalPair.Value >= goalPair.Key.goalValue);
                if (goalSaveData.completed == false)
                {
                    goalSaveData.currentValue = goalPair.Value;
                }

                goalsSaveData.Add(goalSaveData);
            }

            return goalsSaveData;
        }
    }
}