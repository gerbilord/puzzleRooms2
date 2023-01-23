using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIcon : MonoBehaviour
{
    public string displayLevelName;
    public string levelName;
    public string levelLiteral;
    public LevelSelector LevelSelector;
    private void OnGUI()
    {
        GUIStyle largeFont = new GUIStyle();
        largeFont.fontSize = 30;
        largeFont.normal.textColor = Color.white;

        var position = transform.position;
        GUI.Label(new Rect(position.x, position.y, 50, 50), displayLevelName, largeFont);
        
    }

    public void OnMouseUpAsButton()
    {
        LevelSelector.SetupLevel(levelName, levelLiteral);
    }
}
