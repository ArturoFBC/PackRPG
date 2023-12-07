using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game progress to be used for testing while in the editor
[CreateAssetMenu(menuName = "Area/GameProgressEditor")]
public class GameProgressEditor : ScriptableObject
{
    public AreaList areaStates;
}