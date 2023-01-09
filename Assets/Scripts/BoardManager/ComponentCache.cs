using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ComponentCache
{
    public static Dictionary<object, Dictionary<GameObject, object>> Cache = new Dictionary<object, Dictionary<GameObject, object>>();

    // public static Dictionary<GameObject, IGridProperties> GridPropertiesMap = new Dictionary<GameObject, IGridProperties>();
    /*
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
                GridPropertiesMap.Add(gameObjectWithGridProperties, null);
                return null;  
            }
        }
    }
    */
    
    public static T GetItemFromInterfaceCache<T>(GameObject gameObject)
    {
        Type interfaceType = typeof(T);

        if (!Cache.ContainsKey(interfaceType))
        {
            Cache.Add(interfaceType, new Dictionary<GameObject, object>());
        }
        
        Dictionary<GameObject, object> specificCache = Cache[interfaceType];

        bool isCached = specificCache.ContainsKey(gameObject);

        if (isCached)
        {
            return (T)specificCache[gameObject];
        }
        else
        {
            object specificProperty = gameObject.GetComponent(interfaceType);
            
            if (specificProperty != null)
            {
                specificCache.Add(gameObject, specificProperty);
                return (T)specificProperty;
            }
            else
            {
                specificCache.Add(gameObject, null);
                return default;  
            }
        }
    }

    /*
    // Does not return Game Objects with a null component T
    public static Dictionary<GameObject, T> ConvertToDictionaryWithType<T>(List<GameObject> gameObjectsToConvert)
    {
        Dictionary<GameObject, T> convertedObjectsDictionary = new Dictionary<GameObject, T>();

        foreach (var gameObjectToConvert in gameObjectsToConvert)
        {
            if (convertedObjectsDictionary.ContainsKey(gameObjectToConvert))
            {
                Debug.Log("Tried to convert the same gameObject twice.");
                continue;
            }

            T component = GetItemFromInterfaceCache<T>(gameObjectToConvert);
            if (component != null)
            {
                convertedObjectsDictionary.Add(gameObjectToConvert, component);
            }
        }

        return convertedObjectsDictionary;
    }

    // Does not return Game Objects with a null component T or K
    public static Dictionary<T, K> ConvertToDictionaryTypes<T, K>(List<GameObject> gameObjectsToConvert)
    {
        Dictionary<T, K> convertedObjectsDictionary = new Dictionary<T, K>();

        Dictionary<GameObject, T> tDict = ConvertToDictionaryWithType<T>(gameObjectsToConvert);
        Dictionary<GameObject, K> kDict =ConvertToDictionaryWithType<K>(gameObjectsToConvert);

        foreach (var gameObject in gameObjectsToConvert)
        {
            if (tDict.ContainsKey(gameObject) && kDict.ContainsKey(gameObject))
            {
                convertedObjectsDictionary.Add(tDict[gameObject], kDict[gameObject]);
            }
        }
        
        return convertedObjectsDictionary;
    }
    */
    
    public static (GameObject, T, K) ConvertToTypes<T, K>(GameObject gameObject)
    {
        T tType = GetItemFromInterfaceCache<T>(gameObject);
        K kType = GetItemFromInterfaceCache<K>(gameObject);
        
        return (gameObject, tType,kType);
    }
    
    public static List<(GameObject, T, K)> ConvertListToTypes<T, K>(List<GameObject> gameObjectList)
    {
        List<(GameObject, T, K)> returnList = new List<(GameObject, T, K)>();
        
        foreach (var gameObject in gameObjectList)
        {
            var tuple = ConvertToTypes<T, K>(gameObject);
            var (gO, tThing, kThing) = tuple;
            
            if (tThing != null && kThing != null)
            {
                returnList.Add(tuple);
            }
        }

        return returnList;
    }
    public static IGridProperties GetGP(GameObject gameObjectWithGridProperties)
    {
        return GetItemFromInterfaceCache<IGridProperties>(gameObjectWithGridProperties);
    }
}
