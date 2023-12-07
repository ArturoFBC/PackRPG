using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CreatureMovement))]
public class DestinationMarker : MonoBehaviour
{
    public GameObject _DestinationMarkerPrefab;
    private GameObject _DestinationMarker;

    // Start is called before the first frame update
    void Start()
    {
        CreatureMovement myPlayerMovement = GetComponent<CreatureMovement>();

        myPlayerMovement.MoveStartEvent -= OnMovementStart;
        myPlayerMovement.MoveStartEvent += OnMovementStart;
        myPlayerMovement.MoveEndEvent -= OnMovementEnd;
        myPlayerMovement.MoveEndEvent += OnMovementEnd;
    }

    void OnMovementStart(Vector3 destination)
    {
        if (_DestinationMarker == null)
            _DestinationMarker = Instantiate(_DestinationMarkerPrefab);

        _DestinationMarker.transform.position = destination;
        _DestinationMarker.SetActive(true);
    }

    void OnMovementEnd()
    {
        if (_DestinationMarker != null)
            _DestinationMarker.SetActive(false);
    }
}
