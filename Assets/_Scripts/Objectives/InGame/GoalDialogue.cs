using Interactables.Dialogue;
using GameProgress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil.Pdb;

[RequireComponent(typeof(NPC))]
public class GoalDialogue : GoalBehabiourBase
{
    private Dialogue dialogueToListenFor;
    [Header("Dialogue")]
    [SerializeField] private int speechIndexToListenFor;


    protected override void Awake()
    {
        base.Awake();

        dialogueToListenFor = GetComponent<NPC>().GetDialogue();
    }

    private void OnDisable()
    {
        DialogManager.Ref.SpeechDisplayedEvent -= OnSpeechDisplayed;
    }

    protected override void SetCompleted()
    {
        this.enabled = false;
    }

    protected override void SetInProgress()
    {
        Debug.Log($"{goalThisCompletes.id} set in progress");
        DialogManager.Ref.SpeechDisplayedEvent += OnSpeechDisplayed;
    }

    protected override void SetNotStarted()
    {

    }

    private void OnSpeechDisplayed(Dialogue dialogueDisplayed, int speechIndex)
    {
        Debug.Log($"{dialogueDisplayed.name} --- {dialogueToListenFor.name} speech index {speechIndex} --- {speechIndexToListenFor}");
        if (dialogueDisplayed == dialogueToListenFor && speechIndex == speechIndexToListenFor)
        {
            Debug.Log($"{dialogueDisplayed.name} speech index {speechIndex} goal activated");
            OnActivation();
        }
    }
}
