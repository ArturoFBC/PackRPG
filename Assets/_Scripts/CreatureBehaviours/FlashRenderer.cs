using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashRenderer : MonoBehaviour {

    SkinnedMeshRenderer[] _Renderers;
    Material[] _StandardMaterials;

    bool flashingFlag;

    private void Start()
    {
        _Renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        /*
        CreatureLife myLife = GetComponentInParent<CreatureLife>();
        if ( myLife != null )
        {
            myLife.HealthChangedEvent -= Hit;
            myLife.HealthChangedEvent += Hit;
        }*/
    }


    public void Hit(SkillHit hit)
    {
        if (!flashingFlag)
        {
            _StandardMaterials = new Material[_Renderers.Length];

            for ( int i = 0; i < _Renderers.Length; i++ )
            {
                //Save current materials, then swap them
                _StandardMaterials[i] = _Renderers[i].material;
                _Renderers[i].material = IconsAndEffects._Ref.FlashWhenHitMaterial;
            }

            flashingFlag = true;
        }
        else
        {
            CancelInvoke();
        }
        Invoke("EndFlash", 0.05f);
    }

    void EndFlash()
    {
        //Restore the materials previous to the flash
        for (int i = 0; i < _Renderers.Length; i++)
            _Renderers[i].material = _StandardMaterials[i];

        flashingFlag = false;
    }
}
