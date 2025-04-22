using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;


[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    #region Class Variables
    [Header("Components")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;

    [Header("Base Movement")]
    public float runSpeed = 4f;
    public float sprintSpeed = 7f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 1f;
    public float movingThreshold = 0.01f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    private Vector3 lastPosition;
    private Vector3 calculatedVelocity;




    [Header("Camera Settings")]
    public float lookSenseH = 0.1f;
    public float lookSenseV = 0.1f;
    public float lookLimitV = 89f;
    
    private PlayerInput playerInput;
    private PlayerState playerState;

    private Vector2 cameraRotation = Vector2.zero;
    private Vector2 playerTargetRotation = Vector2.zero;

    #endregion

    #region Startup
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerState = GetComponent<PlayerState>();
    }
    #endregion

    #region Update
    private void Update()
    {
        UpdateMovementState();
        UpdateVerticalMovement();
        UpdateLateralMovement();
    }

    private void UpdateVerticalMovement()
    {
        bool isGrounded = IsGrounded();
        Debug.Log("isGrounded: " + isGrounded + ", velocity.y: " + velocity.y);
        if (isGrounded && velocity.y < 0f ) {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        if (playerInput.JumpPressed && isGrounded) { 
            velocity.y = Mathf.Sqrt(jumpHeight * -3f * gravity);
        }
    }

    private void UpdateMovementState() 
    {
        bool isMovementInput = playerInput.MovementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = playerInput.SwitchSprintOn && isMovingLaterally;
        bool isGrounded = IsGrounded();


        if (isGrounded)
        {
            PlayerMovementState lateralState = isSprinting ? PlayerMovementState.Sprinting :
                                               isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;
            playerState.SetPlayerMovementState(lateralState);

        }
        else 
        {
            if (velocity.y > 0f) // Going up
            {
                playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
            }
            else if (velocity.y < 0f) // Falling down
            {
                playerState.SetPlayerMovementState(PlayerMovementState.Falling);
            }
        }
       


    }

 

    private void UpdateLateralMovement() 
    {
       
        bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;

        calculatedVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
  
        Vector2 moveInput = playerInput.MovementInput;

        //right is the red Axis, foward is the blue axis
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float currentSpeed = isSprinting ? sprintSpeed : runSpeed;
        

        characterController.Move(move * currentSpeed * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);

    }
    #endregion

    #region Late Update
    private void LateUpdate()
    {
        cameraRotation.x += lookSenseH * playerInput.LookInput.x;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * playerInput.LookInput.y, -lookLimitV,lookLimitV);
        
        playerTargetRotation.x +=  lookSenseH * playerInput.LookInput.x;
        transform.rotation = Quaternion.Euler(0f,playerTargetRotation.x,0f);

        playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y,cameraRotation.x,0f);

    }
    #endregion

    #region State Checks

    private bool IsMovingLaterally() 
    {
        Vector3 lateralVelocity = new Vector3(calculatedVelocity.x, 0f, calculatedVelocity.z);

        return lateralVelocity.magnitude > movingThreshold;
    }

    private bool IsGrounded()
    {
       return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
    #endregion
}
