using GameProgress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public class GoalInteract : GoalBehabiourBase, IInteractable
    {
        private bool inProgress = false;

        [SerializeField] private string displayName;

        protected override void Awake()
        {
            base.Awake();
        }

        public void Interact(Transform whoActivatedMe)
        {
            if (inProgress)
                OnActivation();
        }

        protected override void SetCompleted()
        {
            gameObject.SetActive(false);
            inProgress = false;
        }

        protected override void SetInProgress()
        {
            inProgress = true;
        }

        protected override void SetNotStarted()
        {
            inProgress = false;
        }

        public string GetDisplayName()
        {
            if (string.IsNullOrEmpty(displayName))
                return gameObject.name;

            return displayName;
        }
    }
}