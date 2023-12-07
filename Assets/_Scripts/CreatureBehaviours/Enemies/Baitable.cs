using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baitable : MonoBehaviour
{
    private bool _Baited = false;

    private void Awake()
    {
        GetComponent<CreatureHitPoints>().KnockOutEvent += OnDeath;
    }

    // Returns whether the bait was successful and the creature has been baited
    public bool BaitCreature( Dictionary<PrimaryStat, float> statIncreases )
    {
        if (_Baited)
            return false;

        UpgradeToAlpha();

        Specimen baitedSpecimen = GetComponent<CreatureStats>().GetSpecimen();

        baitedSpecimen.genotype.RollStats( statIncreases );

        DamageAndPickupsDisplayManager.Ref.DisplayMessage("BAITED", transform.position, Color.red, Color.black, 40);

        _Baited = true;
        return true;
    }

    private void UpgradeToAlpha()
    {
        GetComponent<ExperienceReward>().SetTier(EnemyTier.ALPHA);
        GetComponent<CreatureStats>().UpdateTier(EnemyTier.ALPHA);
        SendMessage("SetTier", EnemyTier.ALPHA);
        transform.localScale = Vector3.one * 1.5f;

        // Rim color depending on tier
        int rimColorPropertyIndex = Shader.PropertyToID("_FresnelColor");

        SkinnedMeshRenderer[] newEnemyRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        List<Material> newEnemyMaterials = new List<Material>();
        foreach (SkinnedMeshRenderer smr in newEnemyRenderers)
            foreach (Material material in smr.materials)
                newEnemyMaterials.Add(material);

        foreach (Material material in newEnemyMaterials)
            material.SetColor(rimColorPropertyIndex, EnemyTierValues.color[(int)EnemyTier.ALPHA]);
    }

    public void OnDeath()
    {
        if ( _Baited )
        {
            Capture();
        }
    }

    public void Capture()
    {
        Specimen mySpecimen = GetComponent<CreatureStats>().GetSpecimen();

        string captureMessage = mySpecimen.name + " HAS JOINED YOUR PACK";
        DamageAndPickupsDisplayManager.Ref.DisplayMessage(captureMessage, transform.position, Color.green, Color.black, 40);

        CreatureStorage.AddSpecimen(mySpecimen, transform.position);

        PackpediaManager.NotifyOwnedSpecies(mySpecimen.species);
    }
}
