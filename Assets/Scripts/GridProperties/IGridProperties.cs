using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridProperties
{
    public int BoardX { get; set; }
    public int BoardY { get; set; }

    public int ActionStart { get; set; }
    public int ActionEnd { get; set; }
    
    public VisualAction CurrentVisualAction { get; set; }
    
    public int eventGroupId { get; set; }

    public bool DoesOccupyTile()
    {
        return false;
    }

    public bool CanPush()
    {
        return false;
    }

    public bool CanBePushed()
    {
        return false;
    }

}
