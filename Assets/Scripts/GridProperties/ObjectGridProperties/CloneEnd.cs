using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class CloneEnd: MonoBehaviour, IGridProperties, IHasCloneId, ICloneEnder, IActivator, IActivatee
{
    public int BoardX { get; set; } = 0;
    public int BoardY { get; set; } = 0;
    
    public int ActionStart { get; set; } = 0;
    public int ActionEnd { get; set; } = 0;

    private bool _isActivated { get; set; } = false;

    public VisualAction CurrentVisualAction { get; set; } = VisualAction.None;
    public List<string> EventGroupIds { get; set; } = new List<string>();
    public int CloneId { get; set; } = -1;
    public void UpdateIsActivated(BoardManager boardManager)
    {
        if (CalculateIfActive(boardManager))
        {
            boardManager.StartNewLevelIteration();
        }
    }

    public bool CalculateIfActive(BoardManager boardManager)
    {
        if (boardManager.CurrentIteration != CloneId)
        {
            return false;
        }
        
        return boardManager.GetLevelGrid().GetTile(BoardX, BoardY).Any(item =>
        {
            var player = ComponentCache.GetItemFromInterfaceCache<IPlayer>(item);

            if (player != null && ComponentCache.GetItemFromInterfaceCache<IHasCloneId>(item).CloneId == CloneId && CloneId != -1)
            {
                return true;
            }

            return false;
        });
    }
}