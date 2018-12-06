using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControlProvider
{

    Vector3 GetMovement();
    Vector3 GetLook();

    bool GetJumpPressed();
    bool GetJumpPressedThisFrame();
    bool GetJumpReleasedThisFrame();

    bool GetMenuPressed();
    bool GetMenuPressedThisFrame();
    bool GetMenuReleasedThisFrame();
}

public class MobileControlProvider : IControlProvider
{
    public Vector3 GetMovement()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 GetLook()
    {
        throw new System.NotImplementedException();
    }

    public bool GetJumpPressed()
    {
        throw new System.NotImplementedException();
    }

    public bool GetJumpPressedThisFrame()
    {
        throw new System.NotImplementedException();
    }

    public bool GetJumpReleasedThisFrame()
    {
        throw new System.NotImplementedException();
    }

    public bool GetMenuPressed()
    {
        throw new System.NotImplementedException();
    }

    public bool GetMenuPressedThisFrame()
    {
        throw new System.NotImplementedException();
    }

    public bool GetMenuReleasedThisFrame()
    {
        throw new System.NotImplementedException();
    }
}
