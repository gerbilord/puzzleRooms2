using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadProperties
{
    public string ObjectName = null; 
    public Color Color = Color.clear;
    public List<string> EventGroupIds = new List<string>();
    public int CloneId = -1;

    public LevelLoadProperties(string rawLevelLoadText)
    {
        string[] parts = rawLevelLoadText.Split(':');
        ObjectName = parts[0].Trim();
        
        foreach (var keyValueEqualPair in parts)
        {
            if (keyValueEqualPair.Contains("="))
            {
                string[] keyValueParts = keyValueEqualPair.Split('=');
                if (keyValueParts.Length == 2)
                {
                    string key = keyValueParts[0].Trim();
                    string value = keyValueParts[1].Trim();

                    if (key.ToLower() == "color" || key.ToLower() == "c")
                    {
                        if(value.ToLower() == "green"){this.Color = Color.green;}
                        if(value.ToLower() == "red"){this.Color = Color.red;}
                        if(value.ToLower() == "yellow"){this.Color = Color.yellow;}
                        if(value.ToLower() == "blue"){this.Color = Color.blue;}
                        if(value.ToLower() == "white"){this.Color = Color.white;}
                    } else if(key.ToLower() == "eventGroup" || key.ToLower() == "eg")
                    {
                        EventGroupIds.Add(value.Trim().ToLower());
                    }
                    else if(key.ToLower() == "cloneid" || key.ToLower() == "spawnorder")
                    {
                        CloneId = Int16.Parse(value);
                    }
                    
                }
            }
        }
    }
}