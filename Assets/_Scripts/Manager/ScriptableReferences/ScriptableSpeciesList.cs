using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Creatures/SpeciesList")]
public class ScriptableSpeciesList : ScriptableObject
{
    public List<Species> species;
}
