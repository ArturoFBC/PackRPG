using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackPediaFiller : MonoBehaviour {

    public GameObject _SpeciesThumbnail;

    public Transform _ThumbnailContainer;

	// Use this for initialization
	void Start ()
    {
		for ( int i = 0; i < ScriptableReferencesHolder.GetSpeciesList().Count; i++)
        {
            SpeciesDisplay st = Instantiate(_SpeciesThumbnail, _ThumbnailContainer).GetComponent<SpeciesDisplay>();
            st.Set(ScriptableReferencesHolder.GetSpeciesReference(i), true);
        }
	}
}
