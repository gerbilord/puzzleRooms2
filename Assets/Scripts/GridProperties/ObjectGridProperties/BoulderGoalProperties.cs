using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoulderGoalProperties : MonoBehaviour, IGridProperties, IFlag, IVictoryCondition
{
    public int BoardX { get; set; } = 0;
    public int BoardY { get; set; } = 0;
    
    public int ActionStart { get; set; } = 0;
    public int ActionEnd { get; set; } = 0;

    public VisualAction CurrentVisualAction { get; set; } = VisualAction.None;
    public List<string> EventGroupIds { get; set; } = new List<string>();

    public bool IsVictoryConditionMet2(BoardManager boardManager)
    {
        var tile = boardManager.GetLevelGrid().GetTile(BoardX, BoardY);
        return tile.Any(item => ComponentCache.GetItemFromInterfaceCache<IBoulder>(item) != null);
    }

    public bool IsVictoryConditionMet(BoardManager boardManager)
    {
        var tile = boardManager.GetLevelGrid().GetTile(BoardX, BoardY);
        
        var bouldersOnTile = tile.Where(item=>ComponentCache.GetItemFromInterfaceCache<IBoulder>(item) != null).ToList();
        
        if (EventGroupIds.Contains(SpecialEventGroups.GENERIC_VICTORY_GROUP))
        {
            return bouldersOnTile.Any();
        }

        return bouldersOnTile.Any(player => ComponentCache.GetGP(player).EventGroupIds.Intersect(EventGroupIds).Any());
    }
}