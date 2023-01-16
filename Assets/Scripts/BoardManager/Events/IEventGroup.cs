using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventGroup
{
    public int EventGroupId { get; set; }
    public void TriggerEventGroup(EventGroupTrigger eventGroupTrigger, BoardManager boardManager);
    public void AddObjectToEventGroup(GameObject gameObjectToAdd);

    public void InitiateState(BoardManager boardManager);
}
