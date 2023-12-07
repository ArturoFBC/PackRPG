using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoamingBehabiour : MonoBehaviour
{
    private enum RoamingState
    {
        STARTING_POSITION,
        GOING,
        ROAMING_POSITION,
        COMING
    }

    [SerializeField] private RoamingState state = RoamingState.STARTING_POSITION;

    [SerializeField] private float waitingCounter;
    [SerializeField] private float waitingTime = 5f;

    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Vector3 roamingPosition;
    [SerializeField] private float roamingDistance = 8f;
    [SerializeField] private CreatureIABasic myBasicIA;

    private void Awake()
    {
        if (myBasicIA == null)
            myBasicIA = GetComponent<CreatureIABasic>();
    }

    private void OnEnable()
    {
        if (startingPosition == default)
            startingPosition = transform.position;

        if (roamingPosition == default)
            roamingPosition = RandomNavmeshLocation(roamingDistance);

        waitingCounter = waitingTime * (0.7f + (0.6f * Random.value));
        MoveNext();
        StartCoroutine(Wait());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitingCounter);

        MoveNext();
    }

    private void MoveNext()
    {
        if ( (transform.position - startingPosition).sqrMagnitude < 1f )
        {
            myBasicIA.MoveCommand(roamingPosition);
        }
        else
        {
            myBasicIA.MoveCommand(startingPosition);
        }
        StartCoroutine(Wait());
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

}
