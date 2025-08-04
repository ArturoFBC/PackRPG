using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameProgress
{
    public class GoalDestroy : GoalBehabiourBase
    {
        [SerializeField] private GameObject destructibleObject;

        protected override void SetCompleted()
        {
            gameObject.SetActive(false);
        }

        protected override void SetInProgress()
        {
            destructibleObject.SetActive(true);
            destructibleObject.GetComponent<CreatureHitPoints>().KnockOutEvent += OnDeath;
        }

        protected override void SetNotStarted()
        {
            destructibleObject.SetActive(false);
        }

        private void OnDeath()
        {
            OnActivation();
        }
    }
}