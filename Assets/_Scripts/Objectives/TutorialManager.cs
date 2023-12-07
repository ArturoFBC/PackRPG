using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class TutorialManager : MonoBehaviour
{
    [Header("StartingPacklings")]
    [SerializeField] private List<Specimen> startingPacklings;

    [Header("Conversations")]
    [SerializeField] private NPC GuideNPC;
    [SerializeField] private Dialogue.Dialogue defeatEnemiesDialogue;
    [SerializeField] private Dialogue.Dialogue healAllyDialogue;

    [Header("Enemies to defeat")]
    [SerializeField] private Specimen enemyToDefeatSpecimen;
    [SerializeField] private int enemyQuantity;
    private List<CreatureHitPoints> enemiesToDefeat = new List<CreatureHitPoints>();
    [SerializeField] Vector3 enemiesSpawnPosition;
    [SerializeField] private GameObject baseEnemy;

    [Header("Ally to heal")]
    [SerializeField] private Specimen allyToHeal;

    private void OnEnable()
    {
        CreatureStorage.activePack.Clear();

        foreach (GameObject creature in GameManager.playerCreatures)
            Destroy(creature);

        GameManager.playerCreatures.Clear();

        foreach (Specimen packling in startingPacklings)
            CreatureStorage.AddSpecimen(packling, Vector3.zero);

        GameManager.SelectPlayer(GameManager.playerCreatures[0]);

        GuideNPC.SetDialogue(defeatEnemiesDialogue);
        DialogManager.Ref.DialogEndedEvent += OnGuideDefeatEnemiesDialogueEnded;
    }

    private void OnGuideDefeatEnemiesDialogueEnded()
    {
        SpawnEnemies();
        DialogManager.Ref.DialogEndedEvent -= OnGuideDefeatEnemiesDialogueEnded;
    }

    public void SpawnEnemies()
    {
        enemiesToDefeat.Clear();

        for (int i = 0; i < enemyQuantity; i++)
        {
            EnemyTier tier = EnemyTier.FAINT;
            GameObject newEnemy = CreatureFactory.CreateCreature(enemyToDefeatSpecimen, baseEnemy, enemiesSpawnPosition, i, tier);

            CreatureHitPoints newEnemyHitPoints = newEnemy.GetComponent<CreatureHitPoints>();
            enemiesToDefeat.Add(newEnemyHitPoints);

            newEnemyHitPoints.KnockOutEvent += OnEnemyKnockOut;
        }
    }

    private void OnEnemyKnockOut()
    {
        bool allDead = true;

        print(allDead);
        foreach (CreatureHitPoints creatureHitPoints in enemiesToDefeat)
        {
            print("BBB");
            if (creatureHitPoints != null && creatureHitPoints.currentHP >= 0)
            {
                print("AAAA");
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            GuideNPC.SetDialogue(healAllyDialogue);
            DialogManager.Ref.DialogEndedEvent += OnGuideHealPackMateDialogueEnded;
        }
    }

    private void OnGuideHealPackMateDialogueEnded()
    {
        SpawnAllyToHeal();
        DialogManager.Ref.DialogEndedEvent -= OnGuideHealPackMateDialogueEnded;
    }

    public void SpawnAllyToHeal()
    {
        CreatureStorage.AddSpecimen(allyToHeal, GuideNPC.transform.position + new Vector3(10f,0f,10f) );

        CreatureHitPoints allyHitPoints = GameManager.playerCreatures[GameManager.playerCreatures.Count - 1].GetComponent<CreatureHitPoints>();

        allyHitPoints.consciousnessRecoveryRate = 0f;

        allyHitPoints.ConsciousnessChangedEvent += AllyHitPoints_ConsciousnessChangedEvent;

        IEnumerator KillPackling()
        {
            yield return new WaitForSeconds(0.2f);
            allyHitPoints.Die();
        }

        StartCoroutine(KillPackling());
    }

    private void AllyHitPoints_ConsciousnessChangedEvent(float health, float maxHealth)
    {
        CreatureHitPoints allyHitPoints = GameManager.playerCreatures[GameManager.playerCreatures.Count - 1].GetComponent<CreatureHitPoints>();

        if (allyHitPoints.consciousness > 0)
        {
            allyHitPoints.RecoverConsciousness(100000);

            allyHitPoints.consciousnessRecoveryRate = 0.1f;
        }
    }
}
