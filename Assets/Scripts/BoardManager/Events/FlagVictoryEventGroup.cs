using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class FlagVictoryEventGroup: IEventGroup
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
        if (eventGroupTrigger != EventGroupTrigger.onMove)
        {
            return;
        }

        List<(GameObject, IGridProperties, IPlayer)> players = ComponentCache.ConvertListToTypes<IGridProperties, IPlayer>(_gameObjectsInEventGroup);
        List<(GameObject, IGridProperties, IFlag)> flags = ComponentCache.ConvertListToTypes<IGridProperties, IFlag>(_gameObjectsInEventGroup);

        foreach (var flagTuple in flags)
        {
            var (flagGameObject, flagGridProperties, flagProperties) = flagTuple;

            var flagVictoryProperties = ComponentCache.GetItemFromInterfaceCache<IVictoryCondition>(flagGameObject);

            flagVictoryProperties.IsVictoryConditionMet = players.Any(playerTuple =>
            {
                var (playerGameObject, playerGridProps, playerProps) = playerTuple;
                return playerGridProps.BoardX == flagGridProperties.BoardX &&
                       playerGridProps.BoardY == flagGridProperties.BoardY;
            });
        }
        
    }
}