using System;
using System.Collections;
using System.Collections.Generic;
using Students.Blaide;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

/// <summary>
/// Just for testing purposes.
/// </summary>
public class ControllerPlus : Controller
{
    public PlayerInput playerInput;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    private void Update()
    {
    
    }

    public void OnLeftStick(InputValue input)
    {
        if (possessable != null)
        {
            possessable.LeftStickAxis(input.Get<Vector2>());
        }
    }
    public void OnRightStick(InputValue input)
    {
        if (possessable != null)
        {
            possessable.RightStickAxis(input.Get<Vector2>());
        }
    }

    public void OnLeftTrigger(InputValue input)
    {
        possessable.gameObject.GetComponent<VehicleSystem>().accelerator = input.Get<float>();
    }

    public void OnRightTrigger(InputValue input)
    {
        possessable.gameObject.GetComponent<VehicleSystem>().breaking = input.Get<float>();
    }

    public void OnActionButton1()
    {
        possessable.OnActionButton1();
    }
}
