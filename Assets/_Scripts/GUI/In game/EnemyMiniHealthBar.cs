using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiniHealthBar : MonoBehaviour
{
    private void Start()
    {
        GetComponent<CreatureHitPoints>().HealthChangedEvent += OnDamage;
    }

    public void OnDamage( float currentHP, float maxHP )
    {
        DamageAndPickupsDisplayManager.Ref.DisplayHealthBar(gameObject);
    }

    public void OnDestroy()
    {
        DamageAndPickupsDisplayManager.Ref.DestroyMiniHealthBar(gameObject);
    }
}
