using UnityEngine;

namespace Fps.UI
{
    public interface IMobileUI
    {
        Vector2 MoveTotal { get; }
        Vector2 LookDelta { get; }
        bool IsAiming { get; }
        bool JumpPressed { get; }
        bool ShootPressed { get; }
        bool ShootHeld { get; }
        bool MenuPressed { get; }
        float MaxStickDistance { get; }
    }
}
