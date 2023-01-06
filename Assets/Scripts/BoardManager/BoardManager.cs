using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CC = ComponentCache;

public class BoardManager : MonoBehaviour
{
    public String levelName;
    [SerializeField] private Transform cameraTransform;
    
    private LevelGrid _levelGrid;
    private List<GameObject> _playerControlledObjects;
    
    public int CurrentTurn { get; private set; }
    void Start()
    {
        CurrentTurn = 0;
        _playerControlledObjects = new List<GameObject>();
        
        LoadLevel();
        InvokeRepeating(nameof(HandleTurn), 1f, 1/Globals.TURNS_PER_SECOND);
        // CancelInvoke to stop
    }

    private void LoadLevel()
    {
        string[][] rawCsvGridData = CsvUtils.CsvToArray(CsvUtils.GetLevelFilePath(levelName));
        int gridSizeX = rawCsvGridData.Length;
        int gridSizeY = rawCsvGridData[0].Length;

        Debug.Log("Board size: x:" +gridSizeX+", y:"+gridSizeY);

        var rawLevelGrid = new List<List<List<GameObject>>>();

        for (int i = 0; i < gridSizeX; i++)
        {
            rawLevelGrid.Add(new List<List<GameObject>>());
            for (int j = 0; j < gridSizeY; j++)
            {
                rawLevelGrid[i].Add(new List<GameObject>());
                
                rawLevelGrid[i][j].AddRange(StringCellToGameObjects(i, j, rawCsvGridData[i][j]));

            }
        }
        
        cameraTransform.position = new Vector3((float)gridSizeX/2 -0.5f, (float)gridSizeY/2 -0.5f, -10);
        _levelGrid = LevelGrid.CreateLevelGrid(rawLevelGrid);
    }

    private GameObject CreateAndSetupGameObject(int x, int y, string resourcePath)
    {
        GameObject newGameObject = Instantiate(Resources.Load(resourcePath)) as GameObject;
        newGameObject.transform.position = new Vector3(x, y);
        ComponentCache.GetGP(newGameObject).BoardX = x;
        ComponentCache.GetGP(newGameObject).BoardY = y;

        return newGameObject;
    }
    private List<GameObject> StringCellToGameObjects(int x, int y, string rawCellText)
    {
        string[] stringItemsInCell = rawCellText.Split(',');

        List<GameObject> gameObjectsInCell = new List<GameObject>();

        foreach (var stringItem in stringItemsInCell)
        {
            GameObject newGameObject = null;
            if (stringItem == "player1")
            {
                newGameObject = CreateAndSetupGameObject(x, y, "GridBoundPrefabs/pf_player1");
                _playerControlledObjects.Add(newGameObject);
            } else if (stringItem == "wall") { newGameObject = CreateAndSetupGameObject(x, y, "GridBoundPrefabs/pf_wall"); } 
            else if (stringItem == "boulder") { newGameObject = CreateAndSetupGameObject(x, y, "GridBoundPrefabs/pf_boulder"); } 
            else if (stringItem == "goal") { newGameObject = CreateAndSetupGameObject(x, y, "GridBoundPrefabs/pf_goal"); } 

            if (newGameObject != null)
            {
                gameObjectsInCell.Add(newGameObject);
            }
        }

        return gameObjectsInCell;

    }

    private List<Actions> GetCurrentActionInputs()
    {
        List<Actions> actions = new List<Actions>();
        List<Actions> noActions = new List<Actions>();
        noActions.Add(Actions.None);

        if (Input.GetKey("up"))
        {
            actions.Add(Actions.Up);
        }
        if (Input.GetKey("down"))
        {
            actions.Add(Actions.Down);
        }
        if (Input.GetKey("left"))
        {
            actions.Add(Actions.Left);
        }
        if (Input.GetKey("right"))
        {
            actions.Add(Actions.Right);
        }
        
        
        return actions.Count == 1 ? actions : noActions;
    }

    private int GetXChange(Actions action)
    {
        int xChange = 0;

        if (action == Actions.Left) { xChange -= 1; }
        if (action == Actions.Right) { xChange += 1; }

        return xChange;
    }
    
    private int GetYChange(Actions action)
    {
        int yChange = 0;
        if (action == Actions.Down) { yChange -= 1; }
        if (action == Actions.Up) { yChange += 1; }

        return yChange;
    }

    private List<RequestedAction> GetRequestedActions()
    {
        List<RequestedAction> requestedActions = new List<RequestedAction>();
        
        List<Actions> currentInputtedActions = GetCurrentActionInputs(); // Currently only returns 1 action;
        Actions inputtedAction = currentInputtedActions[0];

        if (inputtedAction != Actions.None)
        {
            foreach (var playerControlledObject in _playerControlledObjects)
            {
                requestedActions.Add(new RequestedAction(playerControlledObject, inputtedAction));
            }
        }

        return requestedActions;
    }
    
    private void HandleActions()
    {
        List<RequestedAction> requestedActions = GetRequestedActions();
        ExecuteRequestedActions(requestedActions);
    }

    private void ExecuteRequestedActions(List<RequestedAction> requestedActions)
    {
        List<RequestedAction> failedActions = new List<RequestedAction>();
        List<RequestedAction> succeededActions = new List<RequestedAction>();
        
        while (requestedActions.Count > 0)
        {
            RequestedAction currentRequestedAction = requestedActions[0];
            requestedActions.RemoveAt(0);
            
            bool didActionSucceed = AttemptMoveAction(currentRequestedAction) || AttemptPushAction(currentRequestedAction);

            if (didActionSucceed)
            {
                succeededActions.Add(currentRequestedAction);
                requestedActions = failedActions.Concat(requestedActions).ToList();
                failedActions = new List<RequestedAction>();
            }
            else
            {
                failedActions.Add(currentRequestedAction);
            }
        }
    }

    private bool AttemptMoveAction(RequestedAction currentRequestedAction)
    {
        int xChange = GetXChange(currentRequestedAction.requestedAction);
        int yChange = GetYChange(currentRequestedAction.requestedAction);

        if (xChange == 0 && yChange == 0)
        {
            return false; // If no movement inputted, we can't move.
        }

        IGridProperties itemToMove = CC.GetGP(currentRequestedAction.requestingObject);

        int oldX = itemToMove.BoardX;
        int oldY = itemToMove.BoardY;
        int newX = oldX + xChange;
        int newY = oldY + yChange;

        if (!_levelGrid.InBounds(newX, newY))
        {
            return false;  // If movement is off the grid, we can't move.
        }

        if (itemToMove.DoesOccupyTile() && _levelGrid.IsTileOccupied(newX, newY))
        {
            return false; // If the object takes up space, and the new tile is occupied, we can't move into there.
        }
        
        // We made it!
        _levelGrid.rawMoveObject(oldX, oldY, newX, newY, currentRequestedAction.requestingObject);
        return true;
    }
    
    private bool AttemptPushAction(RequestedAction currentRequestedAction)
    {
        int xChange = GetXChange(currentRequestedAction.requestedAction);
        int yChange = GetYChange(currentRequestedAction.requestedAction);

        if (xChange == 0 && yChange == 0)
        {
            return false; // If no movement inputted, we can't move.
        }

        IGridProperties itemToMove = CC.GetGP(currentRequestedAction.requestingObject);

        int oldX = itemToMove.BoardX;
        int oldY = itemToMove.BoardY;
        int newX = oldX + xChange;
        int newY = oldY + yChange;
        int pushedNewX = newX + xChange;
        int pushedNewY = newY + yChange;

        if (!_levelGrid.InBounds(newX, newY) || !_levelGrid.InBounds(pushedNewX, pushedNewY))
        {
            return false;  // If movement is off the grid for us or the item we are pushing.
        }

        List<GameObject> itemsToPush = _levelGrid.GetTile(newX, newY).FindAll(tileObject => CC.GetGP(tileObject).CanBePushed());
        
        if (!itemToMove.CanPush() || itemsToPush.Count == 0)
        {
            return false; // If we can't push or there isn't anything to push, we can't push!
        }

        if (itemsToPush.Exists(itemToPush => CC.GetGP(itemToPush).DoesOccupyTile()) &&
            _levelGrid.IsTileOccupied(pushedNewX, pushedNewY))
        {
            return false; // If any object we are pushing takes up space, and there is no space for it to move to.
        }
        
        // We made it!
        
        // Move pusher.
        _levelGrid.rawMoveObject(oldX, oldY, newX, newY, currentRequestedAction.requestingObject);

        // Move all the items to push.
        foreach (var itemToPush in itemsToPush)
        {
            _levelGrid.rawMoveObject(pushedNewX, pushedNewY, itemToPush);
        }
        
        return true;
    }

    private void HandleTurn()
    {
        HandleActions();
        CurrentTurn++;
    }

    private void Update()
    {
        _levelGrid.forEachCell(
            (x, y, cell) =>
            {
                foreach (var cellItem in cell)
                {
                    cellItem.transform.position = new Vector3(x, y);
                }
                return null;
            }
        );
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10,10,100,100), CurrentTurn.ToString());
    }
}