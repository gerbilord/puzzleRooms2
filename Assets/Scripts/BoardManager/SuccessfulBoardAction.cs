public class SuccessfulBoardAction
{
    public SuccessfulBoardAction(RequestedAction requestedAction, BoardAction boardAction)
    {
        this.RequestedAction = requestedAction;
        this.BoardAction = boardAction;
    }
    public RequestedAction RequestedAction { get; set; }
    public BoardAction BoardAction { get; set; }
}