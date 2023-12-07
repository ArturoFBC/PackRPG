using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    static public int MAX_PACK_SIZE = 4;

    static public GameManager Ref;

    static private bool paused = false;
    static public bool pause
    {
        get { return paused; }
        private set { }
    }

    static public GameObject selectedCreature;
    static public List<GameObject> playerCreatures = new List<GameObject>();

    [SerializeField]
    private Transform startingPosition;
    private string StartingPositionTag = "Respawn";
    [SerializeField]
    private GameObject baseCreature;

    public delegate void PlayerCreatureAdded(int creatureIndex);
    static public event PlayerCreatureAdded playerCreatureAddedEvent;

    public delegate void CreatureSelected(GameObject selectedCreature);
    static public event CreatureSelected creatureSelectedEvent;

    // Use this for initialization
    void Awake()
    {
        if (Ref == null)
            Ref = this;
        else
            DestroyImmediate(this);

        SetStartingPosition();

        SpawnPlayerCreatures();

        if (playerCreatures.Count > 0)
            selectedCreature = playerCreatures[0];

        SceneManager.sceneUnloaded += OnSceneExit;

        SelectPlayer(playerCreatures[0]);
    }

    void Update ()
    {
        ManagePause();
    }

    private static void ManagePause()
    {
        // PAUSE
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
                Unpause();
            else
                Pause();
        }
    }

    private void SetStartingPosition()
    {
        //if (entryPoint == null)
        //    entryPoint = GameObject.FindGameObjectWithTag(entryPointTag).transform;

        if (startingPosition == null)
        {
            startingPosition = (new GameObject("StartingPosition")).transform;
            startingPosition.gameObject.tag = StartingPositionTag;
        }

        startingPosition.position = DataManager._CurrentArea.startingPosition;
    }

    //Get average world position of players, mainly used by camera position
    static public Vector3 GetAveragePlayerPosition()
    {
        Vector3 returnValue = Vector3.zero;

        if (PlayerInput.Ref.controlMode == ControlMode.FOLLOW)
        {
            if (selectedCreature != null)
                return selectedCreature.transform.position;
        }
        else
        {
            if (playerCreatures.Count > 0)
            {
                foreach (GameObject go in playerCreatures)
                    returnValue += go.transform.position;

                returnValue /= playerCreatures.Count;
            }
        }

        return returnValue;
    }

    static public void SelectPlayer( GameObject newSelectedCreature )
    {
        creatureSelectedEvent.Invoke(newSelectedCreature);
        selectedCreature = newSelectedCreature; 
    }
    
    static private void SpawnPlayerCreatures()
    {
        if (playerCreatures != null)
        {
            foreach ( GameObject go in playerCreatures )
            {
                if (go != null)
                    Destroy(go);
            }
            playerCreatures.Clear();
        }

        for ( int creatureIndex = 0; creatureIndex < CreatureStorage.activePack.Count; creatureIndex++ )
        {
            SpawnPlayerCreature(creatureIndex, Ref.startingPosition.position, true);
        }
    }

    static public void SpawnPlayerCreature(int creatureIndex, Vector3 spawnPosition, bool indexBasedOffsetPosition = false)
    {
        Specimen playerCreature = CreatureStorage.activePack[creatureIndex];
        GameObject newPlayerCreature = CreatureFactory.CreateCreature(playerCreature, Ref.baseCreature, spawnPosition, indexBasedOffsetPosition ? creatureIndex : 0 );

        playerCreatures.Add(newPlayerCreature);
        newPlayerCreature.GetComponent<CreatureHitPoints>().KnockOutEvent += OnPlayerCreatureDeath;
        playerCreatureAddedEvent?.Invoke(creatureIndex);
    }

    static public void OnPlayerCreatureDeath()
    {
        bool allDead = true;
        foreach ( GameObject playerCreature in playerCreatures)
        {
            if (playerCreature.GetComponent<CreatureHitPoints>()._Alive == true)
                allDead = false;
        }
        
        ///////////////////////////////////////////////////////////
        //  G A M E   O V E R
        ///////////////////////////////////////////////////////////
        if (allDead)
        {
            InGameGUIManager._Ref.OpenDefeatPanel();

            paused = true;
            Time.timeScale = 0f;
        }
    }

    static public void Pause()
    {
        InGameGUIManager._Ref.OpenPauseMenuPanel();
        if (paused == false)
        {
            paused = true;
            Time.timeScale = 0f;
        }
    }

    static public void Unpause()
    {
        InGameGUIManager._Ref.ClosePauseMenuPanel();
        if (paused == true)
        {
            paused = false;
            Time.timeScale = 1f;
        }
    }

    public void OnSceneExit(Scene scene)
    {
        Unpause();
        SceneManager.sceneUnloaded -= OnSceneExit;
    }
}
