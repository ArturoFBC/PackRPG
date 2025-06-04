using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveLoad;
using System;

namespace GameProgress
{

    public class GameProgressManager : Singleton<GameProgressManager>, ISaveable
    {
        private List<Area> areas;

        private List<Mission> missions;

        private List<Milestone> milestones;

        // Used on the editor to fake the initialization of the data
        [SerializeField]
        private GameProgressEditor gameProgressEditor;

        protected override void InheritedAwake()
        {
            areas = gameProgressEditor.areas;
        }


        public List<Area> GetAreaList()
        {
            return areas;
        }

        public static void Load(GameProgressData data)
        {

        }

        public void Reset()
        {

        }

        internal IEnumerable<KeyValuePair<Area, bool>> GetAllAreasUnlockStatus()
        {
            Dictionary<Area, bool> returnStatus = new Dictionary<Area, bool>();

            List<string> completedMilestones = GetCompletedMilestonesIds();

            for (int i = 0; i < areas.Count; i++)
            {
                bool unlock = true;
                foreach (string milestoneId in areas[i].requiredMilestones)
                {
                    if (completedMilestones.Contains(milestoneId) == false)
                    {
                        unlock = false;
                        break;
                    }
                }

                returnStatus.Add(areas[i], unlock);
            }

            return returnStatus;
        }

        private List<string> GetCompletedMilestonesIds()
        {
            List<Milestone> completedMilestones = milestones.FindAll(x => x.IsCompleted() == true);

            List<string> milestoneIds = new List<string>();
            foreach(Milestone milestone in completedMilestones)
            {
                milestoneIds.Add(milestone.GetID());
            }

            return milestoneIds;
        }

        internal IEnumerable<Mission> GetMissions()
        {
            throw new NotImplementedException();
        }
    }
}
