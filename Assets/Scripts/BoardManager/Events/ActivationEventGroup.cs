using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ActivationEventGroup : IEventGroup, IActivationGroup
{
    public int EventGroupId { get; set; }
    
    private List<GameObject> _gameObjectsInEventGroup = new List<GameObject>();

    public void AddObjectToEventGroup(GameObject gameObjectToAdd)
    {
        if (!_gameObjectsInEventGroup.Contains(gameObjectToAdd))
        {
            _gameObjectsInEventGroup.Add(gameObjectToAdd);
        }
    }

    public void InitiateState(BoardManager boardManager)
    {
        TriggerEventGroup(EventGroupTrigger.onMove, boardManager);
    }

    public void TriggerEventGroup(EventGroupTrigger eventGroupTrigger, BoardManager boardManager)
    {
        if (eventGroupTrigger != EventGroupTrigger.onMove)
        {
            return;
        }
        
        List<IActivatee> activatees = _gameObjectsInEventGroup
            .Select(ComponentCache.GetItemFromInterfaceCache<IActivatee>).NotNull().ToList();
        
        activatees.ForEach(activatee => activatee.UpdateIsActivated(boardManager));
    }

    public bool IsGroupActive(BoardManager boardManager)
    {
        List<IActivator> activators = _gameObjectsInEventGroup
            .Select(ComponentCache.GetItemFromInterfaceCache<IActivator>).NotNull().ToList();

        return activators.Any(item => item.CalculateIfActive(boardManager));
    }
}
