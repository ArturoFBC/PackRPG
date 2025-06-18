using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameProgress
{
    public class GoalDestroy : GoalBehabiourBase
    {
        [SerializeField] private GameObject inertObject, destructibleObject;

        protected override void SetCompleted()
        {
            gameObject.SetActive(false);
        }

        protected override void SetInProgress()
        {
            inertObject.SetActive(false);
            destructibleObject.SetActive(true);
            destructibleObject.GetComponent<CreatureHitPoints>().KnockOutEvent += OnDeath;
        }

        protected override void SetNotStarted()
        {
            inertObject.SetActive(true);
            destructibleObject.SetActive(false);
        }

        private void OnDeath()
        {
            OnActivation();
        }
    }
}