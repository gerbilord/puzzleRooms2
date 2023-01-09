using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ActivationEventGroup : IEventGroup
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

    public void TriggerEventGroup(EventGroupTrigger eventGroupTrigger, BoardManager boardManager)
    {
        if (eventGroupTrigger != EventGroupTrigger.onMove)
        {
            return;
        }

        LevelGrid levelGrid = boardManager.GetLevelGrid();

        List<(GameObject, IGridProperties, IButton)> buttons = ComponentCache.ConvertListToTypes<IGridProperties, IButton>(_gameObjectsInEventGroup);
        List<(GameObject, IGridProperties, IGate)> gates = ComponentCache.ConvertListToTypes<IGridProperties, IGate>(_gameObjectsInEventGroup);

        bool isGroupActivated = false;
        
        foreach (var buttonTuple in buttons)
        {
            var (gameObject, gridInfo, buttonInfo) = buttonTuple;

            var buttonWasPressed = buttonInfo.IsPressed;
            buttonInfo.IsPressed = levelGrid.IsTileOccupied(gridInfo.BoardX, gridInfo.BoardY);

            if (buttonWasPressed != buttonInfo.IsPressed)
            {
                boardManager.AddObjectToAnimate(gameObject);
            }
            
            if (buttonInfo.IsActivated())
            {
                isGroupActivated = true;
            }
        }
        
        foreach (var gateTuple in gates)
        {
            var (gameObject, gridInfo, gateInfo) = gateTuple;
            
            gateInfo.IsActivated = isGroupActivated;
            
            AttemptToOpenGate(gameObject, gridInfo, gateInfo, boardManager);
            AttemptToCloseGate(gameObject, gridInfo, gateInfo, boardManager);
        }
    }

    private void AttemptToCloseGate(GameObject gameObject, IGridProperties gridInfo, IGate gateInfo, BoardManager boardManager)
    {
        if(gateInfo.IsClosed){return;} // Gate already closed. Can't close it.

        if (!boardManager.GetLevelGrid().IsTileOccupied(gridInfo.BoardX, gridInfo.BoardY) && gateInfo.ShouldClose())
        {
            gateInfo.IsClosed = true;
            boardManager.AddObjectToAnimate(gameObject);
        }
    }

    private void AttemptToOpenGate(GameObject gameObject, IGridProperties gridInfo, IGate gateInfo, BoardManager boardManager)
    {
        if (!gateInfo.ShouldClose())
        {
            gateInfo.IsClosed = false;
            boardManager.AddObjectToAnimate(gameObject);
        }
    }
}
