using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallProperties  : MonoBehaviour, IGridProperties
{
    public int BoardX { get; set; } = 0;
    public int BoardY { get; set; } = 0;
    
    public int ActionStart { get; set; } = 0;
    public int ActionEnd { get; set; } = 0;

    public VisualAction CurrentVisualAction { get; set; } = VisualAction.None;
    public List<string> EventGroupIds { get; set; } = new List<string>();

    public bool DoesOccupyTile()
    {
        return true;
    }

    public bool CanBePushed()
    {
        return false;
    }
}