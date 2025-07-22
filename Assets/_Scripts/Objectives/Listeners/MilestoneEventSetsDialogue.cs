using Interactables.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public class MilestoneEventSetsDialogue : MilestoneEventListenerBase
    {
        [Header("Dialogue")]
        [SerializeField] private Dialogue newDialogue;

        [SerializeField] private NPC myNpc;

        private void Awake()
        {
            if (myNpc == null)
            {
                myNpc = GetComponent<NPC>();
            }
        }

        protected override void OnConditionMet()
        {
            myNpc.SetDialogue(newDialogue);
        }
    }
}