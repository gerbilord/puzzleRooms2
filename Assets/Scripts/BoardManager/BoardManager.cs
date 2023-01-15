using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using CC = ComponentCache;

public class BoardManager : MonoBehaviour
{
    #region Properties
    public String levelName;
    private bool _didWin = false;
    private GameObject _winOverlay;
    [SerializeField] private Transform cameraTransform;
    
    private LevelGrid _levelGrid;
    private List<GameObject> _playerControlledObjects;

    private List<IVictoryCondition> _victoryConditions = new List<IVictoryCondition>();

    private List<GameObject> _objectsToAnimateMove;
    private Dictionary<GameObject, float> _objectsToAnimateMoveTimer = new Dictionary<GameObject, float>();

    private Dictionary<int, IEventGroup> _eventGroups = new Dictionary<int, IEventGroup>();
    private List<RequestedEvent> _requestedEvents = new List<RequestedEvent>();
    public int CurrentTurn { get; private set; }
    
    #endregion

    #region PrivateAccessors

    public LevelGrid GetLevelGrid()
    {
        return _levelGrid;
    }

    public void AddObjectToAnimate(GameObject gameObjectToAnimate)
    {
        _objectsToAnimateMove.Add(gameObjectToAnimate);
    }

    #endregion

    #region Startup functions
    void Start()
    {
        CurrentTurn = 0;
        _playerControlledObjects = new List<GameObject>();
        _objectsToAnimateMove = new List<GameObject>();
        _objectsToAnimateMoveTimer = new Dictionary<GameObject, float>();
        
        LoadLevel();
        LoadGUI();
        InvokeRepeating(nameof(HandleTurn), Globals.TIME_BEFORE_STARTING_TURNS, 1/Globals.TURNS_PER_SECOND);
        // CancelInvoke to stop
    }

    private void LoadGUI()
    {
        // _winOverlay = 
        _winOverlay = Instantiate(Resources.Load("GUI/pf_victoryShader")) as GameObject;
        SpriteRenderer winOverlayRenderer = CC.GetItemFromInterfaceCache<SpriteRenderer>(_winOverlay);
        var tempColor = winOverlayRenderer.color;
        tempColor.a = .5f;
        winOverlayRenderer.color = tempColor;
        _winOverlay.transform.position = new Vector3((float)_levelGrid.SizeX()/2 -0.5f, (float)_levelGrid.SizeY()/2 -0.5f, 0);
        _winOverlay.SetActive(false);
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
        
        ComponentCache.GetGP(newGameObject).eventGroupId = -1;

        return newGameObject;
    }

    private List<GameObject> StringCellToGameObjects(int x, int y, string rawCellText)
    {
        string[] stringItemsInCell = rawCellText.Split(',');

        List<GameObject> gameObjectsInCell = new List<GameObject>();

        foreach (var stringItem in stringItemsInCell)
        {
            LevelLoadProperties objectLevelLoadProperties = new LevelLoadProperties(stringItem);
            String itemStringName = objectLevelLoadProperties.ObjectName;

            GameObject newGameObject = null;
            if (itemStringName == "player1") { newGameObject = CreateAndSetupGameObject(x, y, "GridBoundPrefabs/pf_player1"); _playerControlledObjects.Add(newGameObject); } 
            else if (itemStringName == "wall") { newGameObject = CreateAndSetupGameObject(x, y, "GridBoundPrefabs/pf_wall"); } 
            else if (itemStringName == "boulder") { newGameObject = CreateAndSetupGameObject(x, y, "GridBoundPrefabs/pf_boulder"); } 
            else if (itemStringName == "goal") { newGameObject = CreateAndSetupGameObject(x, y, "GridBoundPrefabs/pf_goal"); _victoryConditions.Add(CC.GetItemFromInterfaceCache<IVictoryCondition>(newGameObject));}
            else if (itemStringName == "basicButton") { newGameObject = CreateAndSetupGameObject(x, y, "GridBoundPrefabs/pf_basicButton"); }
            else if (itemStringName == "basicGate") { newGameObject = CreateAndSetupGameObject(x, y, "GridBoundPrefabs/pf_basicGate"); }

            if (newGameObject != null)
            {
                gameObjectsInCell.Add(newGameObject);

                AddObjectToEventGroup(newGameObject, objectLevelLoadProperties.EventGroupId);
                ModifySpriteColor(newGameObject, objectLevelLoadProperties.Color);
            }
        }

        return gameObjectsInCell;
    }

    private void ModifySpriteColor(GameObject newGameObject, Color color)
    {
        if (color != Color.clear) {
            var spriteRenderer = CC.GetItemFromInterfaceCache<SpriteRenderer>(newGameObject);
            spriteRenderer.color = color;
        }
    }

    private void AddObjectToEventGroup(GameObject gameObject, int eventGroupId)
    {
        if (eventGroupId != -1)
        {
            if (!_eventGroups.ContainsKey(eventGroupId))
            {
                var isFlagOrPlayer = CC.GetItemFromInterfaceCache<IFlag>(gameObject) != null || CC.GetItemFromInterfaceCache<IPlayer>(gameObject) != null;
                IEventGroup newGroup;  // JANK this will be dynamic based on a different csv cell, not some case statement
                if (isFlagOrPlayer)
                {
                    newGroup = new FlagVictoryEventGroup();
                }
                else
                {
                    newGroup = new ActivationEventGroup();
                }
                newGroup.EventGroupId = eventGroupId;
                _eventGroups.Add(eventGroupId, newGroup);
            }

            CC.GetGP(gameObject).eventGroupId = eventGroupId;
            _eventGroups[eventGroupId].AddObjectToEventGroup(gameObject);
        }
    }
    
    #endregion

    #region Turn Handling functions

    private void HandleTurn()
    {
        HandleActions();
        HandleEvents();
        HandleVictory();
        CurrentTurn++;
    }

    private void HandleVictory()
    {
        if (_victoryConditions.Count > 0 && !_didWin)
        {
            _didWin = _victoryConditions.TrueForAll(condition => condition.IsVictoryConditionMet);
        }
    }

    private void HandleEvents()
    {
        foreach (var requestedEvent in _requestedEvents)
        {
            var triggeredEvent = requestedEvent.triggeredEvent;
            var triggeredObject = requestedEvent.triggeredObject;
            var eventGroupId = CC.GetGP(triggeredObject).eventGroupId;

            if (eventGroupId != -1)
            {
                _eventGroups[eventGroupId].TriggerEventGroup(triggeredEvent, this);
            }
        }
        _requestedEvents.Clear();
    }

    private void AddTriggerEventsForTile(int x, int y, EventGroupTrigger trigger)
    {
        foreach (var o in _levelGrid.GetTile(x, y))
        {
            AddRequestedEvent(o, trigger);
        }
    }

    #region Helper Functions

    private void AddRequestedEvent(GameObject gameObject, EventGroupTrigger eventGroupTrigger)
    {
        // Fix consolidation of same events or something.
        var requestedEvent = new RequestedEvent
        {
            triggeredObject = gameObject,
            triggeredEvent = eventGroupTrigger
        };

        _requestedEvents.Add(requestedEvent);
    }
    private int GetXChange(InputAction inputAction)
    {
        int xChange = 0;

        if (inputAction == InputAction.Left) { xChange -= 1; }
        if (inputAction == InputAction.Right) { xChange += 1; }

        return xChange;
    }
    
    private int GetYChange(InputAction inputAction)
    {
        int yChange = 0;
        if (inputAction == InputAction.Down) { yChange -= 1; }
        if (inputAction == InputAction.Up) { yChange += 1; }

        return yChange;
    }

    private bool CanObjectAct(GameObject actingObject)
    {
        return CC.GetGP(actingObject).ActionEnd <= CurrentTurn;
    }

    private void AddCooldown(GameObject objectToCooldown, int cooldown)
    {
        CC.GetGP(objectToCooldown).ActionStart = CurrentTurn;
        CC.GetGP(objectToCooldown).ActionEnd = CurrentTurn + cooldown;
    }

    private void CopyCooldown(GameObject objectToCooldown, GameObject objectToCopyCooldown)
    {
        CC.GetGP(objectToCooldown).ActionStart = CC.GetGP(objectToCopyCooldown).ActionStart;
        CC.GetGP(objectToCooldown).ActionEnd = CC.GetGP(objectToCopyCooldown).ActionEnd;
    }
    
    private void AddCooldownWithVisuals(GameObject objectToCooldown, int cooldown, VisualAction visualAction)
    {
        AddCooldown(objectToCooldown, cooldown);
        CC.GetGP(objectToCooldown).CurrentVisualAction = visualAction;
        AddToAnimationList(objectToCooldown);
    }

    private void CopyCooldownWithDifferentVisuals(GameObject objectToCooldown, GameObject objectToCopyCooldown, VisualAction separateVisualAction)
    {
        CopyCooldown(objectToCooldown, objectToCopyCooldown);
        CC.GetGP(objectToCooldown).CurrentVisualAction = separateVisualAction;
        AddToAnimationList(objectToCooldown);
    }


    #endregion

    #region ActionFramework
        private List<InputAction> GetCurrentActionInputs()
    {
        List<InputAction> actions = new List<InputAction>();
        List<InputAction> noActions = new List<InputAction>();
        noActions.Add(InputAction.None);

        if (Input.GetKey("up") || Input.GetKey("w"))
        {
            actions.Add(InputAction.Up);
        }
        if (Input.GetKey("down") || Input.GetKey("s"))
        {
            actions.Add(InputAction.Down);
        }
        if (Input.GetKey("left") || Input.GetKey("a"))
        {
            actions.Add(InputAction.Left);
        }
        if (Input.GetKey("right") || Input.GetKey("d"))
        {
            actions.Add(InputAction.Right);
        }

        return actions.Count == 1 ? actions : noActions;
    }
    
    private List<RequestedAction> GetRequestedActions()
    {
        List<RequestedAction> requestedActions = new List<RequestedAction>();
        
        List<InputAction> currentInputtedActions = GetCurrentActionInputs(); // Currently only returns 1 action;
        InputAction inputtedInputAction = currentInputtedActions[0];

        if (inputtedInputAction != InputAction.None)
        {
            foreach (var playerControlledObject in _playerControlledObjects)
            {
                if (CanObjectAct(playerControlledObject))
                {
                    requestedActions.Add(new RequestedAction(playerControlledObject, inputtedInputAction));
                }
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

    #endregion

    #region ActionAttempters
        private bool AttemptMoveAction(RequestedAction currentRequestedAction)
    {
        int xChange = GetXChange(currentRequestedAction.RequestedInputAction);
        int yChange = GetYChange(currentRequestedAction.RequestedInputAction);

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

        AddCooldownWithVisuals(currentRequestedAction.requestingObject, Globals.TURNS_FOR_BASIC_ACTION, VisualAction.Walking);

        AddTriggerEventsForTile(oldX, oldY, EventGroupTrigger.onMove);
        AddTriggerEventsForTile(newX, newY, EventGroupTrigger.onMove);
        
        return true;
    }
    
    private bool AttemptPushAction(RequestedAction currentRequestedAction)
    {
        int xChange = GetXChange(currentRequestedAction.RequestedInputAction);
        int yChange = GetYChange(currentRequestedAction.RequestedInputAction);

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

        if (!itemsToPush.TrueForAll(CanObjectAct))
        {
            // At least one item is on cooldown and can't be pushed.
            return false;
        }
        
        // We made it!
        // Move pusher.
        _levelGrid.rawMoveObject(oldX, oldY, newX, newY, currentRequestedAction.requestingObject);
        AddCooldownWithVisuals(currentRequestedAction.requestingObject, Globals.TURNS_FOR_HEAVY_PUSH, VisualAction.Pushing);

        // Move all the items to push.
        foreach (var itemToPush in itemsToPush)
        {
            _levelGrid.rawMoveObject(pushedNewX, pushedNewY, itemToPush);
            CopyCooldownWithDifferentVisuals(itemToPush, currentRequestedAction.requestingObject, VisualAction.BeingPushed);
        }
        
        AddTriggerEventsForTile(oldX, oldY, EventGroupTrigger.onMove);
        AddTriggerEventsForTile(newX, newY, EventGroupTrigger.onMove);
        AddTriggerEventsForTile(pushedNewX, pushedNewY, EventGroupTrigger.onMove);
        
        return true;
    }

    #endregion
    
    #endregion

    #region Graphics Functions

    private void AddToAnimationList(GameObject objectToAnimate)
    {
        if (!_objectsToAnimateMove.Contains(objectToAnimate))
        {
            _objectsToAnimateMove.Add(objectToAnimate);
        }
    }

    private void InstantMovement()
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
    private void Update()
    {
        // To debug;
        // InstantMovement();
        // return;

        if (_didWin)
        {
            _winOverlay.SetActive(true);
        }
        
        for (int i = _objectsToAnimateMove.Count - 1; i >= 0 ; i--)
        {
            GameObject objectToAnimate = _objectsToAnimateMove[i];
            Vector2 targetLocation = new Vector2(CC.GetGP(objectToAnimate).BoardX, CC.GetGP(objectToAnimate).BoardY);

            int actionStart = CC.GetGP(objectToAnimate).ActionStart;
            int actionEnd = CC.GetGP(objectToAnimate).ActionEnd;
            int totalTurnsForAction = actionEnd - actionStart;
            int turnsLeft = actionEnd - CurrentTurn;
            int turnsAlreadyPassed = CurrentTurn - actionStart;
            float totalTimeToReachTarget = totalTurnsForAction * Globals.SECONDS_WITHIN_TURN;
            
            // Always update to correct sprite
            ISpriteChanger spriteChanger = CC.GetItemFromInterfaceCache<ISpriteChanger>(objectToAnimate);
            if (spriteChanger != null)
            {
                SpriteRenderer spriteRenderer = CC.GetItemFromInterfaceCache<SpriteRenderer>(objectToAnimate);
                spriteRenderer.sprite = spriteChanger.GetNewSprite();
            }

            // If turn isn't over.
            if (CC.GetGP(objectToAnimate).ActionEnd > CurrentTurn)
            {
                if (!_objectsToAnimateMoveTimer.ContainsKey(objectToAnimate))
                {
                    _objectsToAnimateMoveTimer.Add(objectToAnimate, 0f);
                }

                _objectsToAnimateMoveTimer[objectToAnimate] += Time.deltaTime / totalTimeToReachTarget;
                
                objectToAnimate.transform.position = Vector2.MoveTowards(objectToAnimate.transform.position,
                    targetLocation, Time.deltaTime//_objectsToAnimateMoveTimer[objectToAnimate]  // JANK
                    );
            }
            else
            {
                objectToAnimate.transform.position = targetLocation;
                _objectsToAnimateMove.RemoveAt(i);
                _objectsToAnimateMoveTimer.Remove(objectToAnimate);
            }
        }
    }

    private void OnGUI()
    {
        if (_didWin)
        {
            GUIStyle largeFont = new GUIStyle();
            largeFont.fontSize = 100;
            largeFont.normal.textColor = Color.white;
            GUI.Label(new Rect(Screen.width/2f-250, Screen.height/2f-150, 500, 500), "VICTORY", largeFont);
        }
        GUI.Label(new Rect(20,20,100,100), CsvUtils.GetLevelFilePath(levelName));
        GUI.Label(new Rect(10,10,100,100), CurrentTurn.ToString());
    }

    #endregion

}