using UnityEngine;

public class RequestedAction
{
    public GameObject requestingObject { get; set; }
    public InputAction RequestedInputAction { get; set; }

    public RequestedAction(GameObject requestingObject, InputAction requestedInputAction)
    {
        this.requestingObject = requestingObject;
        this.RequestedInputAction = requestedInputAction;
    }
}
