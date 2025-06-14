using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameProgress
{
    public class GoalDestroy : GoalBehabiourBase
    {
        [SerializeField] private CreatureHitPoints myHitPoints;

        protected override void SetCompleted()
        {
            gameObject.SetActive(false);
        }

        protected override void SetInProgress()
        {
            myHitPoints.enabled = false;
            myHitPoints.KnockOutEvent += OnDeath;
        }

        protected override void SetNotStarted()
        {
            myHitPoints.enabled = false;
        }

        private void OnDeath()
        {
            OnActivation();
        }
    }
}