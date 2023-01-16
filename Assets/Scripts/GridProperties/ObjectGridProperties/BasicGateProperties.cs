using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGateProperties : MonoBehaviour, IGridProperties, ISpriteChanger, IActivatee
{
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closeSprite;
    public int BoardX { get; set; } = 0;
    public int BoardY { get; set; } = 0;
    
    public int ActionStart { get; set; } = 0;
    public int ActionEnd { get; set; } = 0;

    public VisualAction CurrentVisualAction { get; set; } = VisualAction.None;
    public List<int> EventGroupIds { get; set; } = new List<int>();

    public bool DoesOccupyTile()
    {
        return IsClosed;
    }
    public bool IsActivated { get; set; }
    public bool IsClosed { get; set; } = true;
    public virtual bool ShouldClose()
    {
        return !IsActivated;
    }

    public Sprite GetNewSprite()
    {
        return IsClosed ? closeSprite : openSprite;
    }

    public void UpdateIsActivated(BoardManager boardManager)
    {
        var eventGroups = boardManager.GetEventGroups();

        IsActivated = false; // False by default.
        
        foreach (var eventGroupId in EventGroupIds)
        {
            var eventGroup = eventGroups[eventGroupId];

            if (eventGroup is IActivationGroup activationGroup)
            {
                if (activationGroup.IsGroupActive(boardManager))
                {
                    IsActivated = true;
                    break;
                }
            }
        }
        
        // Might as well.
        AttemptToCloseGate(boardManager);
        AttemptToOpenGate(boardManager);
    }

    private void AttemptToCloseGate(BoardManager boardManager)
    {
        if(IsClosed){return;} // Gate already closed. Can't close it.

        if (!boardManager.GetLevelGrid().IsTileOccupied(BoardX, BoardY) && ShouldClose())
        {
            IsClosed = true;
            boardManager.AddObjectToAnimate(gameObject);
        }
    }

    private void AttemptToOpenGate(BoardManager boardManager)
    {
        if (!ShouldClose())
        {
            IsClosed = false;
            boardManager.AddObjectToAnimate(gameObject);
        }
    }
}
