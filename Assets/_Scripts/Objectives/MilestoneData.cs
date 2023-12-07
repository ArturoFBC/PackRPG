using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MilestoneGoalType
{
    GET_ITEM,
    REACH_LOCATION,
    DEFEAT_ENEMY,
    INTERACT
}

public enum MilestoneResultType
{
    ENABLE_OBJECT,
    DISABLE_OBJECT,
    COMPLETE_MISSION,
    PLAY_ANIMATION
}

[System.Serializable]
public class MilestoneGoalData
{
    public MilestoneGoalType goalType;
    public string goalTargetName;
}

[System.Serializable]
public class MilestoneResultData
{
    public MilestoneResultType resultType;
    public string resultTargetName;
}

[System.Serializable]
public class MilestoneData
{
    [SerializeField]
    public List<MilestoneGoalData> goals;

    [SerializeField]
    public List<MilestoneResultData> results;
}
