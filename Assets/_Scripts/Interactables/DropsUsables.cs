using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsUsables : MonoBehaviour
{
    [SerializeField] private GameObject healthGlobePrefab;
    [SerializeField] private GameObject manaGlobePrefab;
    [SerializeField] private GameObject phenotypeBoosterPrefab;

    void Start()
    {
        //Subscribe to death
        GetComponent<CreatureHitPoints>().KnockOutEvent -= OnDeath;
        GetComponent<CreatureHitPoints>().KnockOutEvent += OnDeath;
    }

    private void OnDestroy()
    {
        if (GetComponent<CreatureHitPoints>() != null)
            GetComponent<CreatureHitPoints>().KnockOutEvent -= OnDeath;
    }

    public void OnDeath()
    {
        if (ManaGlobe.WillThisDrop())
        {
            Instantiate(manaGlobePrefab, transform.position, transform.rotation);
        }

        if (HealthGlobe.WillThisDrop())
        {
            Instantiate(healthGlobePrefab, transform.position, transform.rotation);
        }
    }


}
