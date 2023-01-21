using System.Collections.Generic;
using UnityEngine;

class CloneSpawn: MonoBehaviour, IGridProperties, IHasCloneId, ICloneSpawner, IPlayer // Clone spawner is really just a player.
{
public int BoardX { get; set; } = 0;
public int BoardY { get; set; } = 0;
    
public int ActionStart { get; set; } = 0;
public int ActionEnd { get; set; } = 0;

public VisualAction CurrentVisualAction { get; set; } = VisualAction.None;
public List<string> EventGroupIds { get; set; } = new List<string>();
public int CloneId { get; set; } = -1;
}