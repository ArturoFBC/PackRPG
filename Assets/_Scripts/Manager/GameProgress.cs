using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgress : Singleton<GameProgress>
{
    private AreaList _AreaStates;

    /*
    private static List<ProgressMilestone> milestones;
    private static int currentMilestoneIndex;
    */

    /*
    public static void SetCurrentMilestone( int index )
    {
        currentMilestoneIndex = index;
    }
    */

    // Used on the editor to fake the initialization of the data
    [SerializeField]
    private GameProgressEditor gameProgressEditor;

    protected override void InheritedAwake()
    {
        _AreaStates = gameProgressEditor.areaStates;
    }


    public AreaList GetAreaList()
    {
        return _AreaStates;
    }

    public void UnlockArea(Area area)
    {
        if (_AreaStates.ContainsKey(area))
            _AreaStates[area] = true;
        else
            Debug.LogError("Attempt to unlock undefined area: " + area.name);
    }

    public static void Load(GameProgressData data)
    {
        foreach (int areaIndex in data._AreaStates.Keys)
            Ref._AreaStates[ScriptableReferencesHolder.GetAreaReference(areaIndex)] = data._AreaStates[areaIndex];
    }

    public static void Reset()
    {
        //Lock everything
        Ref._AreaStates = new AreaList();
        foreach (AreaUnlock areaUnlock in Resources.FindObjectsOfTypeAll<GameProgressEditor>()[0].areaStates)
            Ref._AreaStates.Add(areaUnlock.area, false);

        //Unlock first area
        Ref._AreaStates[ScriptableReferencesHolder.GetAreaReference(0)] = true;
    }
}
