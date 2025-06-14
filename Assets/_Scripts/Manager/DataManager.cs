using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameProgress;
using SaveLoad;

    public class DataManager : Singleton<DataManager>
    {
        private static Area currentArea;

        public static Area _CurrentArea
        {
            get
            {
                if (reference == null)
                    reference = FindObjectOfType<DataManager>();
                if (currentArea == null)
                    reference.Awake();

                return currentArea;
            }
            private set
            {
                currentArea = value;
            }
        }


        //Development values
        public Area _DevelopmentArea;

        protected override void InheritedAwake()
        {
            DontDestroyOnLoad(gameObject);

            // Development values
            currentArea = _DevelopmentArea;
        }

        public static void SetArea(Area area)
        {
            currentArea = area;
        }

        public static void SaveData()
        {
            SaveLoadManager.Save();
        }

        public static void LoadData()
        {
            SaveGameData data = SaveLoadManager.GetSave();

            if (data != null)
            {
                CreatureStorage.Load(data.creatureStorageData);
                GameProgressManager.Load(data.gameProgressData.missionSaveDatas);
                InventoryManager.Load(data.InventoryManagerData);
                PackpediaManager.Load(data.packpediaData);
            }
            else
                NewData();
        }

        public static void NewData()
        {
            print("NEW DATA");

            CreatureStorage.Ref.Reset();
            GameProgressManager.Ref.Reset();
            InventoryManager.Ref.Reset();
            PackpediaManager.Ref.Reset();
        }
    }