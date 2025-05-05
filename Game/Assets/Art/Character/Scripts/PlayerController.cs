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
    public float RotationMismatch { get; private set; } = 0f;
    public bool IsRotatingToTarget { get; private set; } = false;

    [Header("Base Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float sprintSpeed = 9f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 1f;
    public float movingThreshold = 0.01f;
    Vector3 velocity;

    private Vector3 lastPosition;
    private Vector3 calculatedVelocity;

    [Header("Animation")]
    public float playerModelRotationSpeed = 10f;
    public float rotateToTargetTime = 0.67f;

    [Header("Ground")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;


    [Header("Camera Settings")]
    public float lookSenseH = 0.1f;
    public float lookSenseV = 0.1f;
    public float lookLimitV = 89f;
    
    private PlayerInput playerInput;
    private PlayerState playerState;

    private Vector2 cameraRotation = Vector2.zero;
    private Vector2 playerTargetRotation = Vector2.zero;

    private bool isRotatingClockwise = false;
    private float rotatingToTargetTimer = 0f;

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
        bool canRun = CanRun();
        bool isMovementInput = playerInput.MovementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = playerInput.SwitchSprintOn && isMovingLaterally;
        bool isGrounded = IsGrounded();
        bool isWalking = (isMovingLaterally && !canRun) || playerInput.SwitchWalkOn;

        if (isGrounded)
        {
            PlayerMovementState lateralState = isWalking? PlayerMovementState.Walking :
                                               isSprinting ? PlayerMovementState.Sprinting :
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
        bool isWalking = playerState.CurrentPlayerMovementState == PlayerMovementState.Walking;
        bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;

        calculatedVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
  
        Vector2 moveInput = playerInput.MovementInput;

        //right is the red Axis, foward is the blue axis
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float currentSpeed = isWalking? walkSpeed : isSprinting ? sprintSpeed : runSpeed;
        

        characterController.Move(move * currentSpeed * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);

    }
    #endregion

    #region Late Update
    private void LateUpdate()
    {
        UpdateCameraRotation();

    }

    private void UpdateCameraRotation()
    {
        cameraRotation.x += lookSenseH * playerInput.LookInput.x;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * playerInput.LookInput.y, -lookLimitV, lookLimitV);

        playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * playerInput.LookInput.x;

        float rotationTolerance = 90f;
        bool isIdling = playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
        IsRotatingToTarget = rotatingToTargetTimer > 0;

        // ROTATE if we're not idling
        if (!isIdling)
        {
            RotatePlayerToTarget();
        }
        // If rotation mismatch not within tolerance, or rotate to target is active, ROTATE
        else if (Mathf.Abs(RotationMismatch) > rotationTolerance || IsRotatingToTarget)
        {
            UpdateIdleRotation(rotationTolerance);
        }

        playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);

        // Get angle between camera and player
        Vector3 camForwardProjectedXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 crossProduct = Vector3.Cross(transform.forward, camForwardProjectedXZ);
        float sign = Mathf.Sign(Vector3.Dot(crossProduct, transform.up));
        RotationMismatch = sign * Vector3.Angle(transform.forward, camForwardProjectedXZ);
    }

    private void UpdateIdleRotation(float rotationTolerance)
    {
        // Initiate new rotation direction
        if (Mathf.Abs(RotationMismatch) > rotationTolerance)
        {
            rotatingToTargetTimer = rotateToTargetTime;
            isRotatingClockwise = RotationMismatch > rotationTolerance;
        }
        rotatingToTargetTimer -= Time.deltaTime;

        // Rotate player
        if (isRotatingClockwise && RotationMismatch > 0f ||
            !isRotatingClockwise && RotationMismatch < 0f)
        {
            RotatePlayerToTarget();
        }
    }

    private void RotatePlayerToTarget()
    {
        Quaternion targetRotationX = Quaternion.Euler(0f, playerTargetRotation.x, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationX, playerModelRotationSpeed * Time.deltaTime);
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

    private bool CanRun() 
    {
        return playerInput.MovementInput.y >= Mathf.Abs(playerInput.MovementInput.x);
    }
    #endregion
}
