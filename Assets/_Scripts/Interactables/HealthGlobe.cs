using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthGlobe : PickUp
{
    const float healthPercent = 0.3f;

    public override void Interact(Transform whoActivatedMe)
    {
        CreatureHitPoints activatorHitPoints = whoActivatedMe.GetComponent<CreatureHitPoints>();

        if (activatorHitPoints != null)
        {
            HealHit healHit = new HealHit();
            healHit.baseHeal = activatorHitPoints.MaxHP * healthPercent;

            activatorHitPoints.Heal(healHit);
        }

        DisplayVisualEffects(whoActivatedMe);
        DisplayAudioEffect();
    }

    public static bool WillThisDrop()
    {
        List<float> fillPercent = new List<float>();

        foreach (GameObject go in GameManager.playerCreatures)
        {
            CreatureHitPoints cHP = go.GetComponent<CreatureHitPoints>();
            if (cHP != null)
                fillPercent.Add(cHP.currentHP / cHP.MaxHP);
        }

        return RolePlayingFormulas.HealthManaGlobesDrop(fillPercent);
    }
}
