using System;
using UnityEngine;

public class LevelLoadProperties
{
    public string ObjectName = null; 
    public Color Color = Color.clear;
    public int EventGroupId = -1;

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
                        EventGroupId = Int16.Parse(value);
                    }
                    
                }
            }
        }
    }
}