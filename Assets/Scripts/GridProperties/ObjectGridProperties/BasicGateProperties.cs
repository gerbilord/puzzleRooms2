using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGateProperties : MonoBehaviour, IGridProperties, IGate, ISpriteChanger
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
    public bool ShouldClose()
    {
        return !IsActivated;
    }

    public Sprite GetNewSprite()
    {
        return IsClosed ? closeSprite : openSprite;
    }
}
