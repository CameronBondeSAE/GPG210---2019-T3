using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LucaController : MonoBehaviour
{
    public Possessable possessable;
    public PlayerInput playerInput;
    
    // Start is called before the first frame update
    void Start()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLeftStick(InputAction.CallbackContext context)
    {
        possessable?.LeftStickAxis(context.action.ReadValue<Vector2>());
    }

    public void OnRightStick(InputAction.CallbackContext context)
    {
        possessable?.RightStickAxis(context.action.ReadValue<Vector2>());
    }
}
