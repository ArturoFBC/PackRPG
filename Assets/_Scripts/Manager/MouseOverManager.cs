using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverManager : Singleton<MouseOverManager>
{
    public delegate void MouseOverEvent(GameObject objectUnderCursor);
    static public event MouseOverEvent StartMouseOverEvent;
    static public event MouseOverEvent EndedMouseOverEvent;

    protected static List<string> searchTags = new List<string>() { "Enemy", "Interactable" };

    protected static GameObject objectUnderCursor;

    protected override void InheritedAwake()
    {
        LinkEvents();
    }

    private void OnDestroy()
    {
        UnlinkEvents();
    }

    static private void LinkEvents()
    {
        StartMouseOverEvent += OnStartMouseOver;
        EndedMouseOverEvent += OnEndedMouseOver;
    }

    static private void UnlinkEvents()
    {
        StartMouseOverEvent -= OnStartMouseOver;
        EndedMouseOverEvent -= OnEndedMouseOver;
    }

    static private void OnStartMouseOver(GameObject underCursor)
    {
        switch (underCursor.tag )
        {
            case "Interactable":
                InGameGUIManager._Ref.ShowInteractableInfo(underCursor);
                break;
            case "Enemy":
                InGameGUIManager._Ref.ShowEnemyInfo(underCursor);
                LinkHealth();
                break;
        }
        objectUnderCursor.SendMessage("MouseOverStart", SendMessageOptions.DontRequireReceiver);
    }

    static private void OnEndedMouseOver(GameObject previousObjectUnderCursor)
    {
        switch (previousObjectUnderCursor.tag)
        {
            case "Interactable":
                InGameGUIManager._Ref.ShowEnemyInfo(null);
                break;
            case "Enemy":
                InGameGUIManager._Ref.ShowEnemyInfo(null);
                UnlinkHealth();
                break;
        }
        previousObjectUnderCursor.SendMessage("MouseOverEnded", SendMessageOptions.DontRequireReceiver);
    }

    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

        if (hit.collider == null || searchTags.Contains( hit.collider.tag ) == false )
        {//object gone
            if (objectUnderCursor != null)
            {
                EndedMouseOverEvent?.Invoke(objectUnderCursor);
                objectUnderCursor = null;
            }
        }
        else
        {
            if (objectUnderCursor != null && objectUnderCursor != hit.collider.gameObject)
            {//Different object
                EndedMouseOverEvent?.Invoke(objectUnderCursor);
                objectUnderCursor = hit.collider.gameObject;
                StartMouseOverEvent?.Invoke(objectUnderCursor);
            }
            else if (objectUnderCursor == null)
            {//New object
                objectUnderCursor = hit.collider.gameObject;
                StartMouseOverEvent?.Invoke(objectUnderCursor);
            }
        }
    }

    static void LinkHealth()
    {
        CreatureHitPoints enemyUnderCursorLife = objectUnderCursor.GetComponent<CreatureHitPoints>();
        InGameGUIManager._Ref._MouseOverInfoDisplay._HealthDisplay.SetMaxValue(enemyUnderCursorLife.MaxHP);
        InGameGUIManager._Ref._MouseOverInfoDisplay._HealthDisplay.SetValue(enemyUnderCursorLife.currentHP, enemyUnderCursorLife.MaxHP);
        enemyUnderCursorLife.MaxHealthChangedEvent += InGameGUIManager._Ref._MouseOverInfoDisplay._HealthDisplay.SetMaxValue;
        enemyUnderCursorLife.HealthChangedEvent += InGameGUIManager._Ref._MouseOverInfoDisplay._HealthDisplay.SetValue;
    }

    static void UnlinkHealth()
    {
        CreatureHitPoints enemyUnderCursorLife = objectUnderCursor.GetComponent<CreatureHitPoints>();
        enemyUnderCursorLife.MaxHealthChangedEvent -= InGameGUIManager._Ref._MouseOverInfoDisplay._HealthDisplay.SetMaxValue;
        enemyUnderCursorLife.HealthChangedEvent -= InGameGUIManager._Ref._MouseOverInfoDisplay._HealthDisplay.SetValue;
    }
}
