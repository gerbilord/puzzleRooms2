
public interface IGate
{
    public bool IsActivated { get; set; }
    public bool IsClosed { get; set; }

    public bool ShouldClose();
}
