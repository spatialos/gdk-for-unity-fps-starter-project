using UnityEngine;

public interface IMobileInterface
{
    Vector2 MoveTotal { get; }
    Vector2 LookDelta { get; }
    bool AreAiming { get; }
    bool JumpPressed { get; }
    bool ShootPressed { get; }
    bool AreFiring { get; }
    bool MenuPressed { get; }
}
