using UnityEngine;

public class KeyboardControls : MonoBehaviour, IControlProvider
{
    public Vector3 Movement
    {
        get
        {
            var directionIndex = Forward & !Backward ? 1 : 0;
            directionIndex += Backward & !Forward ? 2 : 0;
            directionIndex += Right & !Left ? 4 : 0;
            directionIndex += Left & !Right ? 8 : 0;
            return cachedDirectionVectors[directionIndex];
        }
    }

    public float YawDelta => Input.GetAxis("Mouse X");
    public float PitchDelta => Input.GetAxis("Mouse Y");
    public bool AreAiming => Input.GetMouseButton(1);
    public bool AreSprinting => Input.GetKey(KeyCode.LeftShift) && Forward && !(Left || Right || Backward);
    public bool JumpPressed => Input.GetKeyDown(KeyCode.Space);
    public bool ShootPressed => Input.GetMouseButtonDown(0);
    public bool ShootHeld => Input.GetMouseButton(0);
    public bool MenuPressed => Input.GetKeyDown(KeyCode.Escape);
    public bool RespawnPressed => Input.GetKeyDown(KeyCode.Space);
    public bool ConnectPressed => Input.GetKeyDown(KeyCode.Space);


    private static bool Forward => Input.GetKey(KeyCode.W);
    private static bool Backward => Input.GetKey(KeyCode.S);
    private static bool Left => Input.GetKey(KeyCode.A);
    private static bool Right => Input.GetKey(KeyCode.D);

    private readonly Vector3[] cachedDirectionVectors = new Vector3[16];

    private void Awake()
    {
        CreateDirectionCache();
    }

    // Cache the direction vectors to avoid having to normalize every time.
    private void CreateDirectionCache()
    {
        for (var i = 0; i < cachedDirectionVectors.Length; i++)
        {
            cachedDirectionVectors[i] = Vector3.zero;
        }

        var forwardRight = new Vector3(1, 0, 1).normalized;
        var forwardLeft = new Vector3(-1, 0, 1).normalized;
        var backwardRight = new Vector3(1, 0, -1).normalized;
        var backwardLeft = new Vector3(-1, 0, -1).normalized;

        cachedDirectionVectors[1] = Vector3.forward;
        cachedDirectionVectors[2] = Vector3.back;
        cachedDirectionVectors[4] = Vector3.right;
        cachedDirectionVectors[5] = forwardRight;
        cachedDirectionVectors[6] = backwardRight;
        cachedDirectionVectors[8] = Vector3.left;
        cachedDirectionVectors[9] = forwardLeft;
        cachedDirectionVectors[10] = backwardLeft;
    }
}
