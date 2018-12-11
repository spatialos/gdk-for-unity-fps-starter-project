using UnityEngine;

// This class is used by MobileControls in place of a real MobileInterface until a valid ref is found
public class MobileInterfaceStandIn : IMobileInterface
{
    public Vector2 MoveTotal { get; }
    public Vector2 LookDelta { get; }
    public bool UpHeld { get; }
    public bool DownHeld { get; }
}
