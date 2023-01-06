using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentCache
{
    public static Dictionary<GameObject, IGridProperties> GridPropertiesMap = new Dictionary<GameObject, IGridProperties>();

    public static IGridProperties GetIGridProperties(GameObject gameObjectWithGridProperties)
    {
        GridPropertiesMap ??= new Dictionary<GameObject, IGridProperties>();

        bool isCached = GridPropertiesMap.ContainsKey(gameObjectWithGridProperties);

        if (isCached)
        {
            return GridPropertiesMap[gameObjectWithGridProperties];
        }
        else 
        {
            IGridProperties gridProperties = gameObjectWithGridProperties.GetComponent<IGridProperties>();
            
            if (gridProperties != null)
            {
                GridPropertiesMap.Add(gameObjectWithGridProperties, gridProperties);
                return gridProperties;
            }
            else
            {
                Debug.Log(gameObjectWithGridProperties.name + " does not have IGridPropertiesComponent");
                return null;  
            }
        }
    }

    public static IGridProperties GetGP(GameObject gameObjectWithGridProperties)
    {
        return GetIGridProperties(gameObjectWithGridProperties);
    }
}
