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
    public float runSpeed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;
    public float movingThreshold = 0.01f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;


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
        UpdateLateralMovement();
    }

    private void UpdateMovementState() 
    {
        bool isMovementInput = playerInput.MovementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();

        PlayerMovementState lateralState = isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;
        playerState.SetPlayerMovementState(lateralState);
    }
    private void UpdateLateralMovement() 
    {
        //checking if we hit the ground to reset our falling velocity, otherwise we will fall faster the next time
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //right is the red Axis, foward is the blue axis
        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * runSpeed * Time.deltaTime);

        //check if the player is on the ground so he can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //the equation for jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

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
        Vector3 lateralVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.y);

        return lateralVelocity.magnitude> movingThreshold;
    }
    #endregion
}
