using UnityEngine;

public class RequestedAction
{
    public GameObject requestingObject { get; set; }
    public Actions requestedAction { get; set; }

    public RequestedAction(GameObject requestingObject, Actions requestedAction)
    {
        this.requestingObject = requestingObject;
        this.requestedAction = requestedAction;
    }
}
