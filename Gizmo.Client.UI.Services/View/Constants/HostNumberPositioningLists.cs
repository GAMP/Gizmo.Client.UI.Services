namespace Gizmo.Client.UI.View.Constants;
/// <summary>
/// Host number positioning lists
/// </summary>
public static class HostNumberPositioningLists
{
    /// <summary>
    /// Clockwise positions
    /// </summary>
    public static readonly List<HostNumberFixedPosition> Clockwise = new()
    {
        HostNumberFixedPosition.TopRight,
        HostNumberFixedPosition.CenterRight,
        HostNumberFixedPosition.BottomRight,
        HostNumberFixedPosition.BottomCenter,
        HostNumberFixedPosition.BottomLeft,
        HostNumberFixedPosition.CenterLeft,
        HostNumberFixedPosition.TopLeft,
        HostNumberFixedPosition.TopCenter
    };
    
    /// <summary>
    /// Counterclockwise positions
    /// </summary>
    public static readonly List<HostNumberFixedPosition> CounterClockwise = new()
    {
        HostNumberFixedPosition.TopRight,
        HostNumberFixedPosition.TopCenter,
        HostNumberFixedPosition.TopLeft,
        HostNumberFixedPosition.CenterRight,
        HostNumberFixedPosition.CenterLeft,
        HostNumberFixedPosition.BottomLeft,
        HostNumberFixedPosition.BottomCenter,
        HostNumberFixedPosition.BottomRight
    };

    /// <summary>
    /// Horizontal top positions
    /// </summary>
    public static readonly List<HostNumberFixedPosition> HorizontalTop = new()
    {
        HostNumberFixedPosition.TopRight,
        HostNumberFixedPosition.TopCenter,
        HostNumberFixedPosition.TopLeft,
        HostNumberFixedPosition.TopCenter
    };
    
    /// <summary>
    /// Horizontal center positions
    /// </summary>
    public static readonly List<HostNumberFixedPosition> HorizontalCenter = new()
    {
        HostNumberFixedPosition.CenterRight,
        HostNumberFixedPosition.CenterScreen,
        HostNumberFixedPosition.CenterLeft,
        HostNumberFixedPosition.CenterScreen
    };
    
    /// <summary>
    /// Horizontal bottom positions
    /// </summary>
    public static readonly List<HostNumberFixedPosition> HorizontalBottom = new()
    {
        HostNumberFixedPosition.BottomRight,
        HostNumberFixedPosition.BottomCenter,
        HostNumberFixedPosition.BottomLeft,
        HostNumberFixedPosition.BottomCenter
    };

    /// <summary>
    /// Vertical right positions
    /// </summary>
    public static readonly List<HostNumberFixedPosition> VerticalRight = new()
    {
        HostNumberFixedPosition.TopRight,
        HostNumberFixedPosition.CenterRight,
        HostNumberFixedPosition.BottomRight,
        HostNumberFixedPosition.CenterRight
    };
    
    /// <summary>
    /// Vertical center positions
    /// </summary>
    public static readonly List<HostNumberFixedPosition> VerticalCenter = new()
    {
        HostNumberFixedPosition.TopCenter,
        HostNumberFixedPosition.CenterScreen,
        HostNumberFixedPosition.BottomCenter,
        HostNumberFixedPosition.CenterScreen
    };
    
    /// <summary>
    /// Vertical left positions
    /// </summary>
    public static readonly List<HostNumberFixedPosition> VerticalLeft = new()
    {
        HostNumberFixedPosition.TopLeft,
        HostNumberFixedPosition.CenterLeft,
        HostNumberFixedPosition.BottomLeft,
        HostNumberFixedPosition.CenterLeft
    };
    
    /// <summary>
    /// Wall bounce positions
    /// </summary>
    public static readonly List<HostNumberFixedPosition> WallBounce = new()
    {
        HostNumberFixedPosition.TopRight,
        HostNumberFixedPosition.BottomCenter,
        HostNumberFixedPosition.TopLeft,
        HostNumberFixedPosition.CenterRight,
        HostNumberFixedPosition.BottomLeft,
        HostNumberFixedPosition.TopCenter,
        HostNumberFixedPosition.BottomRight,
        HostNumberFixedPosition.CenterLeft
    };
}
