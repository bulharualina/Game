using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float blendSpeed = 0.02f;
    [SerializeField] private float attackAnimationDuration = 2.267f;
    private PlayerInput playerInput;
    private PlayerState playerState;
    private PlayerController playerController;

    private static int inputXHash = Animator.StringToHash("inputX");
    private static int inputYHash = Animator.StringToHash("inputY");
    private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");
    private static int isGroundedHash = Animator.StringToHash("isGrounded");
    private static int isJumpingHash = Animator.StringToHash("isJumping");
    private static int isFallingHash = Animator.StringToHash("isFalling");
    private static int isIdlingHash = Animator.StringToHash("isIdling");
    private static int isRotatingToTargetHash = Animator.StringToHash("isRotatingToTarget");
    private static int rotationMismatchHash = Animator.StringToHash("rotationMismatch");
    private static int attackTriggerHash = Animator.StringToHash("AttackTrigger");
    private static int isAttackingHash = Animator.StringToHash("IsAttacking");

    private Vector3 currentBlendInput = Vector3.zero;
    private bool canAttack = true;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerState = GetComponent<PlayerState>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }
    private void OnEnable()
    {
        if (playerInput != null) // Ensure PlayerControls is accessible
        {
            playerInput.OnAttackTriggered -= OnAttackInput;
            playerInput.OnAttackTriggered += OnAttackInput;
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.OnAttackTriggered -= OnAttackInput;
        }
    }
    private void UpdateAnimationState()
    {
        bool isIdling = playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
        bool isRunning = playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
        bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
        bool isGrounded = playerState.InGroundedState();
        bool isJumping = playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
        bool isFalling = playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;


        Vector2 inputTarget = isSprinting ? playerInput.MovementInput * 1.5f :
                                isRunning? playerInput.MovementInput * 1f : playerInput.MovementInput * 0.5f;
        currentBlendInput = Vector3.Lerp(currentBlendInput, inputTarget, blendSpeed * Time.deltaTime);


        animator.SetBool(isGroundedHash,isGrounded);
        animator.SetBool(isJumpingHash,isJumping);
        animator.SetBool(isFallingHash,isFalling);
        animator.SetBool(isIdlingHash, isIdling);
        animator.SetBool(isRotatingToTargetHash, playerController.IsRotatingToTarget);
       

        animator.SetFloat(inputXHash, currentBlendInput.x);
        animator.SetFloat (inputYHash, currentBlendInput.y);
        animator.SetFloat(inputMagnitudeHash, currentBlendInput.magnitude);
        animator.SetFloat(rotationMismatchHash, playerController.RotationMismatch);
      
    }

    private void OnAttackInput()
    {
        if (canAttack)
        {
            animator.SetTrigger(attackTriggerHash);
            animator.SetBool(isAttackingHash, true); 
            canAttack = false; 

            
            StartCoroutine(ResetAttackStateAfterDelay(attackAnimationDuration));
        }
    }
    
    private IEnumerator ResetAttackStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool(isAttackingHash, false); 
        canAttack = true; 
    }


  


}
