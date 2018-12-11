using UnityEngine;

public interface IMobileInterface
{
    Vector2 MoveTotal { get; }
    Vector2 LookDelta { get; }
    bool UpHeld { get; }
    bool DownHeld { get; }

}
