using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTransition : MonoBehaviour
{
    public Area _AreaUnlocked;

    List<GameObject> _CreaturesInside = new List<GameObject>();

    private void Awake()
    {
        transform.position = DataManager._CurrentArea.exitToNextAreaPosition;
        _AreaUnlocked = DataManager._CurrentArea.nextArea;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if ( _CreaturesInside.Contains( other.gameObject ) == false && 
                 GameManager.playerCreatures.Contains( other.gameObject) )
            {
                _CreaturesInside.Add(other.gameObject);

                if (AreAllPlayerCreaturesInside() == true)
                    Activate();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_CreaturesInside.Contains(other.gameObject) == true &&
                 GameManager.playerCreatures.Contains(other.gameObject))
            {
                _CreaturesInside.Remove(other.gameObject);

                if (AreAllPlayerCreaturesInside() == false)
                    Deactivate();
            }
        }
    }

    private bool AreAllPlayerCreaturesInside()
    {
        foreach (GameObject playerCreature in GameManager.playerCreatures)
        {
            if (_CreaturesInside.Contains(playerCreature) == false)
                return false;
        }
        return true;
    }

    private void Activate()
    {
        //Unlock next area
        GameProgress.Ref.UnlockArea(_AreaUnlocked);

        //Show next area panel
        InGameGUIManager._Ref.OpenAreaExitPanel(_AreaUnlocked);
    }

    private void Deactivate()
    {
        InGameGUIManager._Ref.CloseAreaExitPanel();
    }
}
