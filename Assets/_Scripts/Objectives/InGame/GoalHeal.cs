using GameProgress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{

    public class GoalHeal : GoalBehabiourBase
    {
        [SerializeField] private Specimen creatureToHealData;

        [SerializeField] private GameObject baseCreature;

        private GameObject creatureToHeal;

        protected override void SetCompleted()
        {
            gameObject.SetActive(false);
        }

        protected override void SetInProgress()
        {
            creatureToHeal = CreatureFactory.CreateCreature(creatureToHealData, baseCreature, transform.position);

            StartCoroutine("SetUnconscious");
        }

        private IEnumerator SetUnconscious()
        {
            yield return new WaitForEndOfFrame();

            CreatureHitPoints creatureToHealHitPoints = creatureToHeal.GetComponent<CreatureHitPoints>();
            creatureToHealHitPoints.consciousnessRecoveryRate = 0;
            creatureToHealHitPoints.Die();
            creatureToHealHitPoints.RecoverConsciousness(creatureToHealHitPoints.MaxHP - 10f);
            creatureToHealHitPoints.AriseEvent += Complete;
        }

        private void Complete()
        {
            if (creatureToHeal != null)
            {
                Vector3 spawnPosition = creatureToHeal.transform.position;
                Destroy(creatureToHeal);

                CreatureStorage.AddSpecimen(creatureToHealData, spawnPosition);

                PackpediaManager.NotifyOwnedSpecies(creatureToHealData.species);

                OnActivation();
            }
        }

        protected override void SetNotStarted()
        {
            
        }
    }
}