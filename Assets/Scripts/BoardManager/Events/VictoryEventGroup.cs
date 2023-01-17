using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class VictoryEventGroup: IEventGroup, IVictoryCondition
{
    public string EventGroupId { get; set; }
    
    private List<GameObject> _gameObjectsInEventGroup = new List<GameObject>(); // Potential optimization. Only trigger for flags?

    public void AddObjectToEventGroup(GameObject gameObjectToAdd)
    {
        if (!_gameObjectsInEventGroup.Contains(gameObjectToAdd))
        {
            _gameObjectsInEventGroup.Add(gameObjectToAdd);
        }
    }

    public void InitiateState(BoardManager boardManager)
    {
        // NOOP
    }

    public void TriggerEventGroup(EventGroupTrigger eventGroupTrigger, BoardManager boardManager)
    {
        // NOOP
    }

    public bool IsVictoryConditionMet(BoardManager boardManager)
    {
        List<IVictoryCondition> conditions = _gameObjectsInEventGroup
            .Select(ComponentCache.GetItemFromInterfaceCache<IVictoryCondition>).NotNull().ToList();

        return conditions.Count > 0 && conditions.TrueForAll(condition => condition.IsVictoryConditionMet(boardManager) );
    }
}