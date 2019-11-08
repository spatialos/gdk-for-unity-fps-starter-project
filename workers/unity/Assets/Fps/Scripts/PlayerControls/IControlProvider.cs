using UnityEngine;

public interface IControlProvider
{
    Vector3 Movement { get; }
    bool MenuPressed { get; }
    bool RespawnPressed { get; }
    float YawDelta { get; }
    float PitchDelta { get; }
    bool IsAiming { get; }
    bool AreSprinting { get; }
    bool JumpPressed { get; }
    bool ShootPressed { get; }
    bool ShootHeld { get; }
    bool ConnectPressed { get; }
}
