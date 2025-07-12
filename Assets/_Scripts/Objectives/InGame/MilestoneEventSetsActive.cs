using SaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{ 
    public class MilestoneEventSetsActive : MilestoneEventListenerBase
    {
        [Header("Targets")]
        [SerializeField] private List<GameObject> targets;
        [SerializeField] private bool activeStatus;

        protected override void OnConditionMet()
        {
            foreach (GameObject target in targets)
            {
                target.SetActive(activeStatus);
            }
        }

    }
}