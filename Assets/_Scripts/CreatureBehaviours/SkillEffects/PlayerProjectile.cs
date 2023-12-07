using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {

    public float _Speed;
    public SkillHit _MyHit;

    public int _Chain;
    public float _ChainRadius;
    GameObject _LastTarget; //This avoids chained projectiles to hit the same target again

    //Decies wether or not this projectile will go though enemies
    public bool _Pierce;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position += transform.forward * _Speed;
	}

    public void Set( SkillHit skillHit, bool pierce = false )
    {
        _MyHit = skillHit;
        _Pierce = false;

        gameObject.tag = skillHit.attacker.tag;
    }

    public void OnTriggerEnter(Collider other)
    {
        if ( ( (other.tag == "Enemy" && gameObject.tag == "Player") ||
               (other.tag == "Player" && gameObject.tag == "Enemy") )
               && other.gameObject != _LastTarget )
        {
            _MyHit.attackPosition = transform.position;
            other.gameObject.SendMessage("Hit", _MyHit);
            if (_Pierce == false )
            {
                bool chained = false;
                if (_Chain > 0)
                {
                    _Chain--;
                    _LastTarget = other.gameObject;
                    Collider[] posibleTargets = Physics.OverlapSphere(transform.position, _ChainRadius);

                    foreach (Collider c in posibleTargets)
                    {
                        if (((c.tag == "Enemy" && gameObject.tag == "Player") ||
                               (c.tag == "Player" && gameObject.tag == "Enemy"))
                               && c.gameObject != _LastTarget)
                        {
                            for (int i = 0; i < _MyHit.damageInstances.Length; i++)
                                _MyHit.damageInstances[i].damage *= 0.5f;
                            transform.LookAt(c.transform);
                            chained = true;
                            break;
                        }
                    }
                }

                if ( chained == false)
                {
                    SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
                    Destroy(gameObject);
                }
            }
        }
    }
}
