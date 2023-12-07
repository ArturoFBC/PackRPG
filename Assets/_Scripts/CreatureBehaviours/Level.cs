using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField]
    public float exp;

    int preCalculatedLevel;

    public GameObject LevelUpFXGUI;
    public GameObject LevelUpFXWorld;

    public delegate void LevelUpDelegate(int level);
    public event LevelUpDelegate levelUpEvent;

    public static float[] expLevels;
    public static float[] ExpLevels
    {
        get
        {
            if (expLevels == null)
            {
                expLevels = new float[101];
                for (int i = 0; i < expLevels.Length; i++)
                {
                    expLevels[i] = Level.ExperienceFunction(i, true);
                }
            }
            return expLevels;
        }
    }


    public delegate void ExpChanged(float exp, float expForNextLevel);
    public event ExpChanged expChangedEvent;
    public delegate void MaxExpChanged(float exp);
    public event MaxExpChanged maxExpChangedEvent;

    private void Awake()
    {
        preCalculatedLevel = CalculateLevel(exp);
    }

    public int level
    {
        get
        {
            preCalculatedLevel = CalculateLevel(exp);
            return preCalculatedLevel;
        }
    }

    public static int CalculateLevel( float experience )
    {
        return Mathf.FloorToInt( ExperienceFunction(experience));
    }

    /// <summary>
    /// Returns the level for a given exp amount or (if inverse is set) the exp amount necessary to achieve a level
    /// </summary>
    public static float ExperienceFunction(float x, bool inverse = false)
    {
        return Mathf.Pow( x, (inverse ? 3f : 1f / 3f ) );
    }

    public void AddExp(float addedExp)
    {
        exp += addedExp;
        int previousLevel = preCalculatedLevel;
        preCalculatedLevel = CalculateLevel(exp);
        if (previousLevel < preCalculatedLevel)
            LevelUp(preCalculatedLevel);

        GetComponent<CreatureStats>().GetSpecimen().exp = exp;

        expChangedEvent?.Invoke(exp - ExpLevels[preCalculatedLevel], ExpLevels[preCalculatedLevel + 1] - ExpLevels[preCalculatedLevel]);
    }

    public void LevelUp( int newLevel )
    {
        if (LevelUpFXGUI != null)
        {
            Instantiate(LevelUpFXGUI, Camera.main.WorldToScreenPoint( transform.position + new Vector3(0, 6, 0) ), Quaternion.identity, GameObject.Find("GUI").transform);
        }
        if (LevelUpFXWorld != null )
        {
            Instantiate(LevelUpFXWorld, transform.position + new Vector3(0, 4, 0), transform.rotation, transform);
        }

        maxExpChangedEvent?.Invoke(ExpLevels[newLevel + 1] - ExpLevels[newLevel]);
        levelUpEvent?.Invoke(newLevel);
    }

    //Set level without triggering Levelup event.
    public void SetLevel(int level)
    {
        exp = ExperienceFunction((float)level, true);
        preCalculatedLevel = CalculateLevel(exp);
    }
}
