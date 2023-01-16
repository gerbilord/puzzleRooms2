using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BasicButtonProperties : MonoBehaviour, IGridProperties, IActivator
{
    public int BoardX { get; set; } = 0;
    public int BoardY { get; set; } = 0;
    
    public int ActionStart { get; set; } = 0;
    public int ActionEnd { get; set; } = 0;

    public VisualAction CurrentVisualAction { get; set; } = VisualAction.None;
    public List<int> EventGroupIds { get; set; } = new List<int>();

    public bool IsPressed { get; set; }

    public bool IsActivated()
    {
        return IsPressed;
    }

    public bool CalculateIfActive(BoardManager boardManager)
    {
        return boardManager.GetLevelGrid().GetTile(BoardX, BoardY).Any(item=>ComponentCache.GetGP(item).DoesOccupyTile());
    }
}
