using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveOverTime : MonoBehaviour
{
    public  float _DissolveTime;
    private float _DissolveCounter = 0;
    private Material _MyMaterial;
    private int _MaterialPropertyID; 

    private void Start()
    {
        _MyMaterial = GetComponent<Renderer>().material;
        if (_MyMaterial == null)
            enabled = false;

        _MaterialPropertyID = Shader.PropertyToID("_Level");
        _MyMaterial.SetTextureOffset("_NoiseTex", new Vector2(Random.value, Random.value));
    }

    // Update is called once per frame
    void Update ()
    {
        if (_DissolveCounter > _DissolveTime)
            enabled = false;

        _MyMaterial.SetFloat(_MaterialPropertyID, _DissolveCounter / _DissolveTime);
        _DissolveCounter += Time.deltaTime;
	}
}
