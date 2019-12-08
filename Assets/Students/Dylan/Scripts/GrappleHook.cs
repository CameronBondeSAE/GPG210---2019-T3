using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GrappleHook : Possessable
{
    
    [SerializeField]
    private float mouseSensitivity = 1f;
    //[SerializeField]
    //private Transform debugRaycastHit;
    [SerializeField]
    private Transform grappleHookRopeTransform;
    
    private float grappleSpeed;
    [SerializeField] private float grappleSpeedMultiplier = 2f;
    private float grappleJumpMultiplier = 7f;
    private float grappleJumpSpeed = 40f;
    private float reachedHookPositionDistance = 1.5f;
    private float hookRopeLength;
    private float hookThrowSpeed = 70f;

    //needs to be a negative value;
    private float gravityDownForce = -50f;

    private CharacterController characterController;
    private float cameraVerticalAngle;
    private float characterVelocityY;
    private Vector3 characterVelocityMomentum;
    public CinemachineVirtualCamera playerCamera;
    private Vector3 grappleHookHitPosition;

    [SerializeField] private float hookSpeedMin = 10f;
    [SerializeField] private float hookSpeedMax = 40f;

    private Rigidbody rb;

    private State state;

    private enum State
    {   Normal,
        HookThrown,
        UsingGrappleHook

    }



    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        //playerCamera = transform.Find("Camera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
        //Cursor.lockState = CursorLockMode.Locked;
        state = State.Normal;
        //playerCamera = GetComponent<CinemachineVirtualCamera>();
        grappleHookRopeTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        //states for the character
        switch (state)
        {
            default:
            case State.Normal:
                //HandleCharacterLook();
                //HandlePlayerMovement();
                HandleGrappleHook();
                break;
            case State.HookThrown:
                HandleHookThrow();
                //HandleCharacterLook();
                //HandlePlayerMovement();
                break;
            case State.UsingGrappleHook:
                //HandleCharacterLook();
                HandleGrappleHookMovement();
                break;
        }
    }
    
    //handles the characters looking around
    private void HandleCharacterLook()
    {
        float lookX = Input.GetAxisRaw("Mouse X");
        float lookY = Input.GetAxisRaw("Mouse Y");

        //Rotate the transform with around the local y axis
        transform.Rotate(new Vector3(0f, lookX * mouseSensitivity, 0f), Space.Self);

        cameraVerticalAngle -= lookY * mouseSensitivity;
        
        //ensures the camera can't look up too high
        //weird things might start to happen if you do
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);

        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);
    }
    
    //movement handled through the default unity character controller
    private void HandlePlayerMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        float moveSpeed = 20f;

        Vector3 characterVelocity = transform.right * moveX * moveSpeed + transform.forward * moveZ * moveSpeed;
        
        //only allows movement if the player is grounded so you can't move mid air
        if(characterController.isGrounded)
        {
            characterVelocityY = 0f;
            if(TestJumpInput())
            {
                float jumpSpeed = 30f;
                characterVelocityY = jumpSpeed;
            }
        }
        
        //adds a downward force, gravity
        characterVelocityY += gravityDownForce * Time.deltaTime;

        characterVelocity.y = characterVelocityY;

        characterVelocity += characterVelocityMomentum;

        characterController.Move(characterVelocity * Time.deltaTime);

        //dampens the momentum 
        //ensures player doesn't fly off too fast
        if(characterVelocityMomentum.magnitude >= 0f)
        {
            float momentumDrag = 3f;
            characterVelocityMomentum -= characterVelocityMomentum * momentumDrag * Time.deltaTime;
            if(characterVelocityMomentum.magnitude < .0f)
            {
                characterVelocityMomentum = Vector3.zero;
            }
        }
    }

    private void ResetGravity()
    {
        characterVelocityY = 0f;
    }

    private void HandleGrappleHook()
    {
        //shoots a ray at where your pointing like a proper grapple hook
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit raycastHit))
        {
                //debugRaycastHit.position = raycastHit.point;
                grappleHookHitPosition = raycastHit.point;
                hookRopeLength = 0f;
                grappleHookRopeTransform.gameObject.SetActive(true);
                grappleHookRopeTransform.localScale = Vector3.zero;
                state = State.HookThrown;
        }
        
        /*
        if(TestGrappleHookInput())
        {
            if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit raycastHit))
            {
                //debugRaycastHit.position = raycastHit.point;
                grappleHookHitPosition = raycastHit.point;
                hookRopeLength = 0f;
                grappleHookRopeTransform.gameObject.SetActive(true);
                grappleHookRopeTransform.localScale = Vector3.zero;
                state = State.HookThrown;
            }
        }*/
    }

    private void HandleHookThrow()
    {
        //ensures grapple hook gameobject/the rope you see fly out stays in the right spot
        grappleHookRopeTransform.LookAt(grappleHookHitPosition);

        //fakes throwing a rope at the desired position
        //basically just scales up a rope gameobject
        hookRopeLength += hookThrowSpeed * Time.deltaTime;
        grappleHookRopeTransform.localScale = new Vector3(1, 1, hookRopeLength);
        
        //when the visual rope gets to the right position then start to move the player
        if(hookRopeLength >= Vector3.Distance(transform.position, grappleHookHitPosition))
        {
            state = State.UsingGrappleHook; 
        }

    }

    private void HandleGrappleHookMovement()
    {
        //again ensures rope doesn't move while going towards desired position
        grappleHookRopeTransform.LookAt(grappleHookHitPosition);
        Vector3 hookDirection = (grappleHookHitPosition - transform.position).normalized;
        
        //clamps the speed you move towards the desired location between a max and min value
        //its because the grapple hook is fast when moving initially but slows down the
        //closer to the desired position you get
        grappleSpeed = Mathf.Clamp(Vector3.Distance(transform.position, grappleHookHitPosition), hookSpeedMin, hookSpeedMax);
        
        //move to hook location
        //characterController.Move(hookDirection * grappleSpeed * grappleSpeedMultiplier * Time.deltaTime);
        rb.AddForce(grappleSpeedMultiplier * grappleSpeed * hookDirection);
        //rb.velocity = hookDirection * grappleSpeed * grappleSpeedMultiplier;
        
        //this allows the player to cancel the grapple hook movement mid move
        if (Vector3.Distance(transform.position, grappleHookHitPosition) < reachedHookPositionDistance)
        {
            StopGrappleHook();
        }
        if(TestGrappleHookInput())
        {
            StopGrappleHook();
        }

        if (TestJumpInput())
        {
            characterVelocityMomentum = hookDirection * grappleSpeed * grappleJumpMultiplier;
            characterVelocityMomentum += Vector3.up * grappleJumpSpeed;
            state = State.Normal;
            ResetGravity();
            StopGrappleHook();
        }
    }
    
    //resets visible grapple hook
    //also resets the player
    private void StopGrappleHook()
    {
        //Cancel GrappleHook
        state = State.Normal;
        ResetGravity();
        grappleHookRopeTransform.gameObject.SetActive(false);
        
    }

    //the grapples controls
    private bool TestGrappleHookInput()
    {
        return Input.GetKeyDown(KeyCode.F);
    }

    private bool TestJumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    
    //this is for if it is added into the game
    public override void OnActionButton1()
    {
        TestGrappleHookInput();
        base.OnActionButton1();
    }
}
