using UnityEngine;
using System.Collections;

public class InstantiateOnDeath : MonoBehaviour {
	
	public GameObject _ToBeInstantiated;	
	
	public void OnDeath()
	{
			DelayedDeath();
	}
		
	void DelayedDeath()
	{
		if ( _ToBeInstantiated != null )
		{
			GameObject instantiatedEffect = Instantiate( _ToBeInstantiated, transform.position, transform.rotation ) as GameObject;
			instantiatedEffect.transform.localScale = gameObject.transform.localScale;
		}
	}

	public void ChangeObjectToInstantiateOnDeat( GameObject deathEffect )
	{
		_ToBeInstantiated = deathEffect;
	}
}
