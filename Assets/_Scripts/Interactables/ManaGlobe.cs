using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaGlobe : PickUp
{
    const float manaPercernt = 0.3f;

    public override void Interact(Transform whoActivatedMe)
    {
        CreatureMana activatorEnergyPoints = whoActivatedMe.GetComponent<CreatureMana>();

        if (activatorEnergyPoints != null)
        {
            activatorEnergyPoints.AddMana(activatorEnergyPoints.MaxMP * manaPercernt);
        }

        DisplayVisualEffects(whoActivatedMe);
        DisplayAudioEffect();
    }

    public static bool WillThisDrop()
    {
        List<float> fillPercent = new List<float>();

        foreach (GameObject go in GameManager.playerCreatures)
        {
            CreatureMana cm = go.GetComponent<CreatureMana>();
            if (cm != null)
                fillPercent.Add(cm.currentMP / cm.MaxMP);
        }

        return RolePlayingFormulas.HealthManaGlobesDrop(fillPercent);
    }
}
