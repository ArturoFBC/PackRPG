using GameProgress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaveLoad
{
    [System.Serializable]
    public class GameProgressData
    {
        [System.Serializable]
        struct GoalSaveData
        {
            public string id;
            public int currentValue;
            public bool completed;
        }

        [System.Serializable]
        struct MilestoneSaveData
        {
            public string id;
            public List<GoalSaveData> goalCurrentValues;
            public bool completed;
        }

        [System.Serializable]
        struct MissionSaveData
        {
            public string id;
            public List<MilestoneSaveData> milestoneCurrentValues;
            public bool completed;
        }


        public GameProgressData(GameProgressManager gameProgress)
        {
            List<MissionSaveData> missionSaveDatas = new List<MissionSaveData>();

            foreach (Mission mission in gameProgress.GetMissions())
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
                    missionSaveData.milestoneCurrentValues = GetMilestoneSaveData(mission.GetMilestones());
                }

                missionSaveDatas.Add(missionSaveData);
            }
        }

        private static List<MilestoneSaveData> GetMilestoneSaveData(IEnumerable<Milestone> milestones)
        {
            List<MilestoneSaveData> milestonesSaveData = new List<MilestoneSaveData>();
            foreach (Milestone milestone in milestones)
            {
                MilestoneSaveData milestoneSaveData = new MilestoneSaveData();
                milestoneSaveData.id = milestone.GetID();
                milestoneSaveData.completed = milestone.IsCompleted();
                if (milestoneSaveData.completed)
                {
                    milestoneSaveData.goalCurrentValues = new List<GoalSaveData>();
                }
                else
                {
                    milestoneSaveData.goalCurrentValues = GetGoalCurrentValues(milestone.GetGoalDatas());
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