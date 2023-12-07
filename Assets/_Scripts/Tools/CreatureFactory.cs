using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CreatureFactory
{
    public static GameObject CreateCreature(Specimen specimen, GameObject baseCreature, Vector3 startPosition, int index = 0, EnemyTier tier = EnemyTier.STANDARD)
    {
        Vector3 position = PositionTools.GetSpiralPosition(startPosition, index);
        GameObject newCreature = GameObject.Instantiate(baseCreature, position, Quaternion.identity);

        GameObject newCreatureModel = GameObject.Instantiate(specimen.species.model, newCreature.transform.position, newCreature.transform.rotation, newCreature.transform);
        newCreatureModel.AddComponent<CreatureSoundChild>();

        newCreature.GetComponent<Level>().SetLevel(Level.CalculateLevel(specimen.exp));

        newCreature.GetComponent<CreatureStats>().SetSpecimen(specimen, tier);

        newCreature.SendMessage("SetTier", tier, SendMessageOptions.DontRequireReceiver);

        newCreatureModel.transform.localScale = EnemyTierValues.sizeMultiplier[(int)tier] * Vector3.one;

        // Rim color depending on tier
        SetRimColor(EnemyTierValues.color[(int)tier], newCreatureModel);

        return newCreature;
    }

    private static void SetRimColor(Color rimColor, GameObject newCreatureModel)
    {
        SkinnedMeshRenderer[] newCreatureRenderers = newCreatureModel.GetComponentsInChildren<SkinnedMeshRenderer>();
        List<Material> newCreatureMaterials = new List<Material>();
        foreach (SkinnedMeshRenderer smr in newCreatureRenderers)
            foreach (Material material in smr.materials)
                newCreatureMaterials.Add(material);

        int rimColorPropertyIndex = Shader.PropertyToID("_RimColor");
        foreach (Material material in newCreatureMaterials)
            material.SetColor(rimColorPropertyIndex, rimColor);
    }
}
