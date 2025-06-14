using GameProgress;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public class GoalTrigger : GoalBehabiourBase
    {
        private Collider myCollider;

        protected override void Awake()
        {
            myCollider = GetComponent<Collider>();

            base.Awake();
        }

        protected override void SetNotStarted()
        {
            myCollider.enabled = false;
        }

        protected override void SetInProgress()
        {
            myCollider.enabled = true;
        }

        protected override void SetCompleted()
        {
            SetNotStarted();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.GetComponentInChildren<CreatureIABasic>() != null)
                {
                    OnActivation();
                }
            }
        }
    }
}