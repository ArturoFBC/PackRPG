using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerShotEffect : MonoBehaviour {

    public ParticleSystem _MyParticles;
    public LineRenderer _MyLineRenderer;

    public float _Duration;
    float _Counter;

    float _InitialWidth;

    private void Start()
    {
        //_InitialWidth = _MyLineRenderer.wis
    }

    // Update is called once per frame
    void Update ()
    {
        _MyLineRenderer.widthMultiplier = (1 - (_Counter / _Duration)) * 0.3f;
        _MyParticles.transform.position = new Vector3(_MyParticles.transform.position.x, _MyParticles.transform.position.y,_MyParticles.transform.position.z + _Counter * 10f);
        _Counter += Time.deltaTime;
	}
}
