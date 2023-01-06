using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderProperties  : MonoBehaviour, IGridProperties
{
    public int BoardX { get; set; }
    public int BoardY { get; set; }

    public bool DoesOccupyTile()
    {
        return true;
    }

    public bool CanBePushed()
    {
        return true;
    }
}