using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using CC = ComponentCache;



public class LevelSelector : MonoBehaviour
{
    public string defaultLevelName;
    private string defaultLevelLiteral = LevelLiterals.pokemonLevel; // ADD FOR ONLINE BUILDS 
    [SerializeField] private Transform cameraTransform;
    private BoardManager _currentLevel; 
    void Start()
    {
        SetupLevel(defaultLevelName, defaultLevelLiteral);
    }

    private void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            SetupLevel(_currentLevel.levelName, _currentLevel.levelLiteral);
        } else if (Input.GetKeyDown("1"))
        {
            SetupLevel(defaultLevelName, LevelLiterals.pokemonLevel);
        } else if (Input.GetKeyDown("2"))
        {
            SetupLevel(defaultLevelName, LevelLiterals.boulderGateLevel);
        } else if (Input.GetKeyDown("3"))
        {
            SetupLevel(defaultLevelName, LevelLiterals.cloneTestLevel);
        } else if (Input.GetKeyDown("4"))
        {
            SetupLevel(defaultLevelName, LevelLiterals.doublePlayerSwitchLevel);
        } else if (Input.GetKeyDown("5"))
        {
            SetupLevel(defaultLevelName, LevelLiterals.aBitOfEverything);
        } else if (Input.GetKeyDown("6"))
        {
            SetupLevel(defaultLevelName, LevelLiterals.sokobanBoulderLevel);
        } else if (Input.GetKeyDown("7"))
        {
            SetupLevel(defaultLevelName, LevelLiterals.myLevel1);
        } else if (Input.GetKeyDown("8"))
        {
            SetupLevel(defaultLevelName, LevelLiterals.myLevel2);
        } else if (Input.GetKeyDown("9"))
        {
            SetupLevel(defaultLevelName, LevelLiterals.pokemonLevel);
        } else if (Input.GetKeyDown("0"))
        {
            SetupLevel(defaultLevelName, LevelLiterals.pokemonLevel);
        }
    }

    public void SetupLevel(string levelName, string levelLiteral)
    {
        if (_currentLevel != null)
        {
            Destroy(_currentLevel.GameObject()); // CONSIDER CLEANING COMPONENT CACHE
        }

        GameObject newLevelGameObject = Instantiate(Resources.Load("pf_boardManager")) as GameObject;
        _currentLevel = CC.GetItemFromInterfaceCache<BoardManager>(newLevelGameObject);
        _currentLevel.levelName = levelName;
        _currentLevel.levelLiteral = levelLiteral;
        _currentLevel.cameraTransform = cameraTransform;
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(20,40,1000,100), "Press r to restart. Change level by pressing 0-9");
    }
}
