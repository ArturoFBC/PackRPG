using System;
using UnityEngine;

namespace GameProgress
{
    [CreateAssetMenu(menuName = "GameProgress/GoalData")]
    [System.Serializable]
    public class GoalData : ScriptableObject
    {
        public string id;
        public string description;
        public int goalValue;
    }
}