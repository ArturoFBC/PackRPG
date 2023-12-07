using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class Drop
{
    public Droppable dropable;
    public int amount = 1;
}

public class PickUpItem : PickUp
{
    [SerializeField] private Drop myDrop;

    [SerializeField] private Renderer itemRenderer;
    [SerializeField] private Renderer lineRenderer;
    [SerializeField] private DecalProjector lighProjector;
    [SerializeField] private MeshFilter myMeshFilter;

    public void Set( Drop drop )
    {
        myDrop.dropable = drop.dropable;
        myDrop.amount = drop.amount;

        gameObject.name = myDrop.dropable.GetName();

        Color color = myDrop.dropable.dropColor;
        color.a *= 0.7f;

        SetRendererColor(itemRenderer, color);
        color.a = 0.3f;
        SetRendererColor(lineRenderer, color );
        SetMaterialColor(lighProjector.material, color );

        if (myMeshFilter != null && myDrop.dropable.dropMesh != null )
        {
            myMeshFilter.mesh = myDrop.dropable.dropMesh;
        }

        if (drop.dropable.autoPickUp)
            SetAutoPickUp(AUTO_PICKUP_RADIUS);
    }

    public void Set(Drop drop, Vector3 destination)
    {
        Set(drop);

        ArcIntoPosition myArcIntoPosition = GetComponent<ArcIntoPosition>();
        if (myArcIntoPosition != null)
        {
            myArcIntoPosition.Set(destination);
        }
    }

    public override void Interact( Transform whoActivatedMe )
    {
        AddDropToInventory( myDrop );

        DisplayVisualEffects(whoActivatedMe);
        DisplayAudioEffect();
        DisplayMessage();

        Destroy(this);
    }

    private void AddDropToInventory(Drop myDrop)
    {
        myDrop.dropable.AddToInventory(myDrop.amount);
    }

    private void SetRendererColor( Renderer thisRenderer, Color color )
    {
        foreach (Material material in thisRenderer.materials)
        {
            SetMaterialColor(material, color);
        }
    }

    private static void SetMaterialColor(Material material, Color color )
    {
        material.SetColor("_BaseColor", color);
        material.SetColor("_Color", color);
    }

    protected void DisplayMessage()
    {
        if (DamageAndPickupsDisplayManager.Ref != null) DamageAndPickupsDisplayManager.Ref.DisplayPickup(myDrop.amount, myDrop.dropable.GetName(), transform.position);
    }
}
