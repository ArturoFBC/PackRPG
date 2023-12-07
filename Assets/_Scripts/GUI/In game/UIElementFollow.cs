using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElementFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _Following;

    public Vector3 _RelativePosition = new Vector3(0f,3f,0f);

    public bool _HideAutomaticly = true;
    public float _Duration = 3f;
    private float _DurationCounter;

    public void SetFollowing( Transform following )
    {
        _Following = following;
        _DurationCounter = _Duration;
    }

    private void LateUpdate()
    {
        if ( _HideAutomaticly && (_DurationCounter < 0 || _Following == null) )
        {
            gameObject.SetActive(false);
        }
        else
        {
            transform.position = Camera.main.WorldToScreenPoint( _Following.position + _RelativePosition);
            _DurationCounter -= Time.deltaTime;
        }

    }

    public void RefreshDuration ()
    {
        gameObject.SetActive(true);
        _DurationCounter = _Duration;
    }
}
