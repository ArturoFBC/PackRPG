using SaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    // Game progress to be used for testing while in the editor
    [CreateAssetMenu(menuName = "GameProgress/GameProgressEditor")]
    public class GameProgressEditor : ScriptableObject
    {
        public List<MissionSaveData> gameProgressData;
    }
}