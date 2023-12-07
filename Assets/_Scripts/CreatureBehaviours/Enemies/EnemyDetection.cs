using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    EnemyIA _MyEnemyIA;

    private void Awake()
    {
        if (_MyEnemyIA == null)
            _MyEnemyIA = GetComponentInParent<EnemyIA>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<CreatureStats>() != null)
            _MyEnemyIA.TargetDetected(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<CreatureStats>() != null)
            _MyEnemyIA.TargetLost(other.gameObject);
    }
}
