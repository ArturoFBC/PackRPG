using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameProgress
{
    public class MilestoneEventSwapMusic : MilestoneEventListenerBase
    {
        protected override void OnConditionMet()
        {
            MusicManager._Ref.StartMusic(MusicTrackType.TENSION);
        }
    }
}