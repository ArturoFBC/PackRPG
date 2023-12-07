using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBoltEffect : MonoBehaviour
{
    public Material _MyMaterial;

    public LineRenderer _MainLine;
    public GameObject _SecondaryLinePrefab;
    public LineRenderer[] _SecondaryLines;
    public GameObject _BeginMarker;
    public GameObject _EndMarker;

    public Vector3 _StartPosition;
    public Vector3 _EndPosition;

    public int _MainLineBendPerUnit = 1;
    public int _SecondaryLineBends = 5;
    public float _RandomAmplitude = 0.2f;

    public AnimationCurve _ColorCurve;
    public float _Duration = 1f;
    public float _RemainingTime = 0;

    void Start()
    {
        _MyMaterial = GetComponent<LineRenderer>().material;
    }


    public void Set( Vector3 startPostion, Vector3 endPosition )
    {
        _StartPosition = startPostion;
        _EndPosition = endPosition;
    }

    void OnEnable ()
    {
        _RemainingTime = _Duration;

        _MainLine.positionCount = Mathf.CeilToInt( (_EndPosition - _StartPosition).magnitude * _MainLineBendPerUnit );
        if (_MainLine.positionCount < 2) _MainLine.positionCount = 2;

        if (_SecondaryLines == null || _SecondaryLines.Length != _MainLine.positionCount - 2)
            _SecondaryLines = new LineRenderer[ Mathf.Clamp(_MainLine.positionCount - 2, 0 , _MainLine.positionCount - 2 )];

        Vector3 step = (_EndPosition - _StartPosition) / (_MainLine.positionCount - 1);
        Vector3[] basePositions = new Vector3[_MainLine.positionCount ];
        for ( int i = 0; i < _MainLine.positionCount; i++)
        {
            Vector3 newPosition = _StartPosition + (step * i);
            basePositions[i] = newPosition;
            if (i != 0 && i != _MainLine.positionCount - 1)
            {
                newPosition.x += (Random.value * _RandomAmplitude) - (_RandomAmplitude / 2);
                newPosition.y += (Random.value * _RandomAmplitude) - (_RandomAmplitude / 2);
            }
            _MainLine.SetPosition( i, newPosition );
        }

        float secondaryStepLength = step.sqrMagnitude / 3f;
        if (_SecondaryLines != null && _SecondaryLines.Length > 0)
        {
            for (int l = 0; l < _SecondaryLines.Length; l++)
            {
                if (_SecondaryLines[l] == null)
                    _SecondaryLines[l] = Instantiate(_SecondaryLinePrefab, transform).GetComponent<LineRenderer>();

                //if (Random.value < 0.5f)
                {
                    _SecondaryLines[l].enabled = true;
                    _SecondaryLines[l].positionCount = _SecondaryLineBends;
                    Vector3 secondaryStep = ( (basePositions[l+1] - _MainLine.GetPosition(l+1) ).normalized + (step.normalized / 2f) )* secondaryStepLength;

                    for (int i = 0; i < _SecondaryLines[l].positionCount; i++)
                    {
                        Vector3 newPosition = _MainLine.GetPosition(l + 1) + (secondaryStep * i);
                        if (i != 0)
                        {
                            newPosition.x += (Random.value * _RandomAmplitude/2) - (_RandomAmplitude / 4);
                            newPosition.y += (Random.value * _RandomAmplitude/2) - (_RandomAmplitude / 4);
                            newPosition.z += (Random.value * _RandomAmplitude/2) - (_RandomAmplitude / 4);
                        }
                        _SecondaryLines[l].SetPosition(i, newPosition);
                    }
                }
                //else
                //    _SecondaryLines[l].enabled = false;
            }
        }

        _BeginMarker.transform.position = _StartPosition;
        _EndMarker.transform.position = _EndPosition;

        //InvokeRepeating("RedrawEffect", 0f, 0.05f);
    }

    private void Update()
    {
        _RemainingTime -= Time.deltaTime;
        float currentAlpha = _ColorCurve.Evaluate((_Duration-_RemainingTime)/_Duration);
        foreach (LineRenderer lr in GetComponentsInChildren<LineRenderer>())
        {
            lr.startColor = new Color(lr.startColor.r, lr.startColor.g, lr.startColor.b, currentAlpha);
            if ( lr == _MainLine)
                lr.endColor = new Color(lr.endColor.r, lr.endColor.g, lr.endColor.b, currentAlpha);
        }
    }

    void RedrawEffect()
    {
        Vector3 step = (_EndPosition - _StartPosition) / (_MainLine.positionCount - 1);
        Vector3[] basePositions = new Vector3[_MainLine.positionCount];
        for (int i = 0; i < _MainLine.positionCount; i++)
        {
            Vector3 newPosition = _StartPosition + (step * i);
            basePositions[i] = newPosition;
            if (i != 0 && i != _MainLine.positionCount - 1)
            {
                newPosition.x += (Random.value * _RandomAmplitude) - (_RandomAmplitude / 2);
                newPosition.y += (Random.value * _RandomAmplitude) - (_RandomAmplitude / 2);
            }
            _MainLine.SetPosition(i, newPosition);
        }

        float secondaryStepLength = step.sqrMagnitude / 2f;
        if (_SecondaryLines != null && _SecondaryLines.Length > 0)
        {
            for (int l = 0; l < _SecondaryLines.Length; l++)
            {
                if (Random.value < 0.5f)
                {
                    _SecondaryLines[l].enabled = true;
                    Vector3 secondaryStep = (_MainLine.GetPosition(l) - basePositions[l]).normalized * secondaryStepLength;

                    for (int i = 0; i < _SecondaryLines[l].positionCount; i++)
                    {
                        Vector3 newPosition = _MainLine.GetPosition(l + 1) + (secondaryStep * i);
                        if (i != 0)
                        {
                            newPosition.x += (Random.value * _RandomAmplitude) - (_RandomAmplitude / 2);
                            newPosition.y += (Random.value * _RandomAmplitude) - (_RandomAmplitude / 2);
                            newPosition.z += (Random.value * _RandomAmplitude) - (_RandomAmplitude / 2);
                        }
                        _SecondaryLines[l].SetPosition(i, newPosition);
                    }
                }
                else
                    _SecondaryLines[l].enabled = false;
            }
        }

        _MyMaterial.SetTextureOffset("_MainTex", new Vector2(Random.value, 0f));
    }
}
