using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoalProperties : MonoBehaviour, IGridProperties, IFlag, IVictoryCondition
{
    public int BoardX { get; set; } = 0;
    public int BoardY { get; set; } = 0;
    
    public int ActionStart { get; set; } = 0;
    public int ActionEnd { get; set; } = 0;

    public VisualAction CurrentVisualAction { get; set; } = VisualAction.None;
    public List<string> EventGroupIds { get; set; } = new List<string>();

    public bool IsVictoryConditionMet(BoardManager boardManager)
    {
        var tile = boardManager.GetLevelGrid().GetTile(BoardX, BoardY);
        
        var playersOnTile = tile.Where(item=>ComponentCache.GetItemFromInterfaceCache<IPlayer>(item) != null).ToList();
        
        if (EventGroupIds.Contains(SpecialEventGroups.GENERIC_VICTORY_GROUP))
        {
            return playersOnTile.Any();
        }

        return playersOnTile.Any(player => ComponentCache.GetGP(player).EventGroupIds.Intersect(EventGroupIds).Any());
    }
}