using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum FontType
{
    SIMPLE,
    ORNATE
}

public class IconsAndEffects : MonoBehaviour {

    [CreatureStats]
    public Sprite[] baseStats = new Sprite[(int)PrimaryStat.END];

    public Sprite[] damageTypes = new Sprite[Enum.GetValues(typeof(DamageType)).Length];
    public Color[] damageTypeColors = new Color[Enum.GetValues(typeof(DamageType)).Length];
    public Color[] statColors = new Color[Enum.GetValues(typeof(PrimaryStat)).Length];
    public Sprite test;

    public GameObject BurnEffect;
    public GameObject FreezeEffect;
    public Material FreezeMaterial;
    public GameObject ShockEffect;
    public GameObject ItemDropping;

    public Material FlashWhenHitMaterial;

    public Font simpleFont;
    public Font ornateFont;

    [Header("Minimap")]
    public Sprite playerMapMarkUnselected;
    public Sprite playerMapMarkSelected;
    public Sprite exitMapMark;

    public static IconsAndEffects _Ref;

    private void Awake()
    {
        if (_Ref == null)
            _Ref = this;
    }
}
