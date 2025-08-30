using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityLogManager : MonoBehaviour
{
    public static EntityLogManager Instance;
    public List<EntityLog> entityLogs = new List<EntityLog>();
    public PlayerStats playerStats;

    private void Awake()
    {
        Instance= this;
    }
    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    public EntityLog GetEntityLog(string name) 
    {
        foreach (var log in entityLogs)
        {
            if (log.entityName == name) 
            {
                return log;
            }
        }
        return null;
    }

    public void ChangeCritChance(float chance) {
        if(playerStats==null) playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        if (playerStats != null) { 
            playerStats.UpdateBaseCritChance(chance);
        }
    }

}

[Serializable]
public class EntityLog
{
    public string entityName;
    public float dialogueCount;
    public List<LogProgress> logProgress = new List<LogProgress>();

    float currentNextObjective=10;

    public void CheckObjectiveComplete()
    {
        if (logProgress.Count > 0)
        {
            foreach (LogProgress log in logProgress)
            {
                
                if (!log.isObjectiveComplete)
                {
                    if (dialogueCount >= log.dialogueObjective)
                    {
                        log.isObjectiveComplete = true;
                        log.onObjectiveComplete?.Invoke();
                        currentNextObjective = log.dialogueObjective+10;
                        UpdateDialogueNumber();
                        break;
                    }
                    currentNextObjective = log.dialogueObjective;
                    break;
                }
            }
        }
    }
    public void IncrementDialogueCount(float amount)
    {
        dialogueCount += amount;
        CheckObjectiveComplete();
    }

    public void UpdateDialogueNumber()
    {
        GameManagerDD.instance.dialogueNumber.text = $"{dialogueCount}/{currentNextObjective}";
    }
}
[Serializable]
public class LogProgress
{ 
    public float dialogueObjective;
    public bool isObjectiveComplete = false;
    public UnityEvent onObjectiveComplete;
}
