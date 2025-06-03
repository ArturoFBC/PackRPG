using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

public abstract class MilestoneGoalBase
{
    protected MilestoneGoalData myGoalData;

    protected bool done = false;

    public delegate void GoalAchieved();
    public event GoalAchieved GoalAchievedEvent;

    public static MilestoneGoalBase CreateMilestone(MilestoneGoalData data)
    {
        MilestoneGoalBase milestoneGoal = null;

        foreach (Type type in
            Assembly.GetAssembly(typeof(MilestoneGoalBase)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(MilestoneGoalBase))))
        {
            MilestoneGoalBase mgb = (MilestoneGoalBase)Activator.CreateInstance(type);
            if (mgb.GetGoalType() == data.goalType)
            {
                milestoneGoal = mgb;
                milestoneGoal.ActivateGoal(data);
                break;
            }
        }

        if (milestoneGoal == null)
            Debug.LogError("Attempting to create an Objective Milestone Goal with a MilestoneGoalType with no matching MilestoneGoal implementation.");

        return milestoneGoal;
    }

    protected void CallGoalAchievedEvent()
    {
        GoalAchievedEvent.Invoke();
    }

    public abstract MilestoneGoalType GetGoalType();
    public void ActivateGoal(MilestoneGoalData data)
    {
        myGoalData = data;
        ActivateGoal();
    }

    protected abstract void ActivateGoal();

    public abstract bool CheckGoal();
}

public class MilestoneGoalDefeatEnemy : MilestoneGoalBase
{
    private GameObject enemyToDefeat;

    public override MilestoneGoalType GetGoalType()
    {
        return MilestoneGoalType.DEFEAT_ENEMY;
    }

    protected override void ActivateGoal()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyToDefeat = Array.Find(enemies, (x) => x.name == myGoalData.goalTargetName);

        CreatureHitPoints enemyHitPoints = enemyToDefeat.GetComponent<CreatureHitPoints>();
        enemyHitPoints.KnockOutEvent += EnemyHitPoints_KnockOutEvent;
    }

    ~MilestoneGoalDefeatEnemy()
    {
        if (enemyToDefeat != null)
        {
            CreatureHitPoints enemyHitPoints = enemyToDefeat.GetComponent<CreatureHitPoints>();
            if (enemyHitPoints != null)
                enemyHitPoints.KnockOutEvent -= EnemyHitPoints_KnockOutEvent;
        }
    }

    private void EnemyHitPoints_KnockOutEvent()
    {
        done = true;
        CallGoalAchievedEvent();
    }

    public override bool CheckGoal()
    {
        return done;
    }
}
