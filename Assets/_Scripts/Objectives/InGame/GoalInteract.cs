using GameProgress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public class GoalInteract : GoalBehabiourBase
    {
        private IInteractable interactable;

        protected override void Awake()
        {
            interactable = GetComponent<IInteractable>();

            base.Awake();
        }

        protected override void SetCompleted()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetInProgress()
        {
        }

        protected override void SetNotStarted()
        {
            throw new System.NotImplementedException();
        }

    }
}