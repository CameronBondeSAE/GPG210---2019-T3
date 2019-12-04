using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Possessable possessable;
    public PlayerInfo playerInfo;

    private void Start()
    {
        //possessable = FindObjectOfType<Possessable>();
    }
    private void Update()
    {
        /*if (possessable != null)
            possessable.LeftStickAxis(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));*/
    }
    
    
    public void OnLeftStick(InputValue input)
    {
        if (possessable != null)
            possessable.LeftStickAxis(input.Get<Vector2>());
        
    }
    public void OnRightStick(InputValue input)
    {
        if (possessable != null)
            possessable.RightStickAxis(input.Get<Vector2>());
        
    }

    public void OnLeftTrigger(InputValue input)
    {
        if (possessable != null)
            possessable.LeftTrigger(input.Get<float>());
    }

    public void OnRightTrigger(InputValue input)
    {
        if (possessable != null)
            possessable.RightTrigger(input.Get<float>());
    }

    public void OnActionButton1()
    {
        if (possessable != null)
            possessable.OnActionButton1();
    }

    public void OnEnterExitButton()
    {
        
    }
}
