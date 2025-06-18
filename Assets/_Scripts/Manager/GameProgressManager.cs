using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveLoad;
using System;

namespace GameProgress
{

    public class GameProgressManager : Singleton<GameProgressManager>, ISaveable
    {
        [SerializeField] private ScriptableAreaList areaList;

        [SerializeField] private List<MissionData> missionsData;

        private static List<Mission> missionsState;

        #region CACHED_PROGRESS
        private static List<Milestone> activeMilestones;
        private static List<MilestoneData> completedMilestones;
        private static Dictionary<GoalData, MilestoneData> milestoneContainsGoal;
        private static Dictionary<MilestoneData, MissionData> missionContainsMilestone;
        #endregion

        // Used on the editor to fake the initialization of the data
        [SerializeField]
        private GameProgressEditor gameProgressEditor;

        public Action<MilestoneData, MilestoneStatus> MilestoneUpdatedEvent;

        protected override void InheritedAwake()
        {
            Load(gameProgressEditor.gameProgressData);
        }

        public static void Load(List<MissionSaveData> missionSaveDatas)
        {
            FillReverseLookupDictionaries(Ref.missionsData);

            missionsState = new List<Mission>();

            foreach (MissionData missionData in Ref.missionsData)
            {
                MissionSaveData missionSaveData = missionSaveDatas.Find( x => x.id == missionData.id );

                Mission mission;
                if (string.IsNullOrEmpty( missionSaveData.id) )
                {
                    Debug.Log($"{missionSaveData.id} was not found.");
                    mission = new Mission(missionData);
                    missionsState.Add(mission);
                    continue;
                }

                Debug.Log($"{missionSaveData.id} was found.");
                mission = new Mission(missionData, missionSaveData);
                missionsState.Add(mission);
            }

            RebuildCachedData();
        }

        public void Reset()
        {
            FillReverseLookupDictionaries(missionsData);

            missionsState = new List<Mission>();

            foreach (MissionData missionData in missionsData)
            {
                Mission mission = new Mission(missionData);
                missionsState.Add(mission);
            }

            RebuildCachedData();
        }

        private static void RebuildCachedData()
        {
            activeMilestones = GetActiveMilestones(missionsState);
            completedMilestones = GetCompletedMilestones(missionsState);
        }

        private static void FillReverseLookupDictionaries(List<MissionData> missionsData)
        {
            if (milestoneContainsGoal != null)
            {
                return;
            }

            milestoneContainsGoal = new Dictionary<GoalData, MilestoneData>();
            missionContainsMilestone = new Dictionary<MilestoneData, MissionData>();

            foreach (MissionData missionData in missionsData)
            {
                foreach (MilestoneData milestoneData in missionData.GetMilestones())
                {
                    missionContainsMilestone.Add(milestoneData, missionData);
                    foreach (GoalData goalData in milestoneData.GetGoals())
                    {
                        milestoneContainsGoal.Add(goalData, milestoneData);
                    }
                }
            }
        }

        private static List<Milestone> GetActiveMilestones(List<Mission> missions)
        {
            activeMilestones = new List<Milestone>();
            foreach (Mission mission in missions)
            {
                Milestone milestone = mission.GetCurrentMilestone();
                if (milestone != null)
                    activeMilestones.Add(milestone);
            }
            return activeMilestones;
        }

        private static List<MilestoneData> GetCompletedMilestones(List<Mission> missions)
        {
            List<MilestoneData> milestones = new List<MilestoneData>();

            foreach (Mission mission in missions)
            {
                foreach (MilestoneData milestone in mission.GetCompletedMilestones())
                {
                    milestones.Add(milestone);
                }
            }

            return milestones;
        }



        public static void ReportGoal(GoalData goal, int increasedAmount)
        {
            MilestoneData milestoneData = milestoneContainsGoal[goal];
            MissionData missionData = missionContainsMilestone[milestoneData];

            bool found = false;
            foreach (Mission mission in missionsState)
            {
                if (mission.GetData() == missionData)
                {
                    mission.ReportMilestoneGoalProgress(milestoneData, goal, increasedAmount);
                    found = true;
                    break;
                }
            }

            if (found == false)
            {
                Debug.LogWarning(string.Format("Attempted to report progress in Goal {0} , but no active milestone contains that goal", goal.id));
            }
        }

        public static bool IsMilestoneCompleted(MilestoneData milestone)
        {
            return completedMilestones.Contains(milestone);
        }

        public static bool IsMilestoneActive(MilestoneData milestone)
        {
            foreach (Mission mission in missionsState)
            {
                if (mission.ContainsMilestone(milestone) == false)
                    continue;

                if (mission.GetCurrentMilestone().GetData() == milestone)
                    return mission.GetCurrentMilestone().myStatus == MilestoneStatus.IN_PROGRESS;
            }

            return false;
        }

        internal static IEnumerable<KeyValuePair<Area, bool>> GetAllAreasUnlockStatus()
        {
            Dictionary<Area, bool> returnStatus = new Dictionary<Area, bool>();

            for (int i = 0; i < Ref.areaList.areas.Count; i++)
            {
                bool unlock = true;
                foreach (MilestoneData milestone in Ref.areaList.areas[i].requiredMilestones)
                {
                    if (completedMilestones.Contains(milestone) == false)
                    {
                        unlock = false;
                        break;
                    }
                }

                returnStatus.Add(Ref.areaList.areas[i], unlock);
            }

            return returnStatus;
        }

        internal static IEnumerable<Mission> GetMissions()
        {
            return missionsState;
        }

        internal static MilestoneStatus GetMilestoneStatus(GoalData goal)
        {
            if (milestoneContainsGoal == null)
                return MilestoneStatus.NOT_STARTED;

            MilestoneData milestoneData = milestoneContainsGoal[goal];

            if (milestoneData == null)
            {
                Debug.LogWarning("Queried about goal <" + goal.id + ">, that is not in the GameProgressManager cache.");
                return MilestoneStatus.NOT_STARTED;
            }

            foreach (Mission mission in missionsState)
            {
                if (mission.ContainsMilestone(milestoneData) == false)
                    continue;

                if (mission.GetCurrentMilestone().GetData() == milestoneData)
                {
                    return mission.GetCurrentMilestone().myStatus;
                }
            }

            Debug.LogWarning("Queried about goal <" + goal.id + ">, belonging to milestone <" + milestoneData.id +
                             ">, and said milestone could not be found amongst active milestones." );
            return MilestoneStatus.NOT_STARTED; 
        }
    }
}
