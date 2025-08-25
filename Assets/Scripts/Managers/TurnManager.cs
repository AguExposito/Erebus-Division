using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;
    public List<EntityInterface> entitiesTurns = new List<EntityInterface>();
    public float encounterThreathLevel;
    private void Awake()
    {
        instance = this;
    }

    public void AddTurn(EntityInterface entity) {
        if (entitiesTurns.Contains(entity)) return;
        entitiesTurns.Add(entity);
        encounterThreathLevel=UpdateThreathLevel();
    }
    public void RemoveTurn(EntityInterface entity)
    {
        if (!entitiesTurns.Contains(entity)) return;
        entitiesTurns.Remove(entity);
        encounterThreathLevel = UpdateThreathLevel();
    }
    public void EndTurn(EntityInterface entity) {
        if (!entitiesTurns.Contains(entity) || entitiesTurns.IndexOf(entity)!=0) return;
        entitiesTurns.Add(entitiesTurns[0]);
        entitiesTurns.RemoveAt(0);
        entitiesTurns[0].isItsTurn = true;
        if (entitiesTurns.Count > 1)
        {
            entitiesTurns.FindLast(e => e != entitiesTurns[0]).isItsTurn = false;
        }
        StartTurn(entitiesTurns[0]);
    }
    public void StartTurn(EntityInterface entity)
    {
        if (!entitiesTurns.Contains(entity) || !entity.isItsTurn) return;
        if (entity.TryGetComponent<ManThing>(out ManThing manThing))
        {
            manThing.CheckTurn();
        }
    }

    public float UpdateThreathLevel() {
        float tempTlvl=0;
        foreach (var entity in entitiesTurns)
        {
            tempTlvl+= entity.threathLevel;
        }
        return tempTlvl;
    }
}
