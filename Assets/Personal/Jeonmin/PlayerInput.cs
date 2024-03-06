using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public Vector2 moveAxis;
    public Vector2 mouseAxis;

    public void OnMove(InputValue value)
    {
        moveAxis = value.Get<Vector2>();
    }
    public void OnLook(InputValue value)
    {
        mouseAxis = value.Get<Vector2>();
    }

    public void OnFire(InputValue value)
    {

    }
}
