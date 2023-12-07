using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamageObject : MonoBehaviour {

    public float _Duration;
    public SkillHit _Hit;
    public string _TargetTag;
    public float _Radius;

	// Use this for initialization
	public void Set( float duration, SkillHit hit, string targetTag, float radius )
    {
        _Duration = duration;
        _Hit = hit;
        _TargetTag = targetTag;
        _Radius = radius;

        Destroy(gameObject, _Duration);
        InvokeRepeating("SendDamage", 0f, 1f);
        transform.localScale *= _Radius;

        DestroyInXseconds selfdestruct = gameObject.AddComponent<DestroyInXseconds>();
        selfdestruct.timeToBeDestroyed = duration;
	}

    void SendDamage()
    {
        Collider[] posibleTargets = Physics.OverlapSphere(transform.position, _Radius);

        foreach (Collider c in posibleTargets)
        {
            if ( _TargetTag == c.tag )
            {
                c.SendMessage("Hit", _Hit);
            }
        }
    }

}
