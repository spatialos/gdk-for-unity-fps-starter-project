using UnityEngine;

// This class is used by MobileControls in place of a real MobileInterface until a valid ref is found
public class MobileUIStandIn : IMobileUI
{
    public Vector2 MoveTotal { get; }
    public Vector2 LookDelta { get; }
    public bool AreAiming { get; }
    public bool JumpPressed { get; }
    public bool ShootPressed { get; }
    public bool AreFiring { get; }
    public bool MenuPressed { get; }
}
