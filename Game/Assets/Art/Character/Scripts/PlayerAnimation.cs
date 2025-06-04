using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float blendSpeed = 0.02f;
    [SerializeField] public Collider axeHitbox; // Assign your axe's trigger collider here in the Inspector!
    [SerializeField] private float hitCooldown = 0.5f; // NEW: Adjust this value in Inspector (e.g., 0.2f to 0.5f)
    private bool canRegisterHit = true;

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
        if (playerInput != null) 
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

            
            StartCoroutine(ResetAttackStateAfterDelay());
        }
    }
    
    private IEnumerator ResetAttackStateAfterDelay()
    {
        yield return new WaitForSeconds(1);
        animator.SetBool(isAttackingHash, false); 
        canAttack = true; 
    }
    public void EnableAxeCollider()
    {
        if (axeHitbox != null)
        {
            axeHitbox.enabled = true;
            canRegisterHit = true; // Reset for a new swing
            Debug.Log("Axe collider ENABLED");
        }
    }
    public void TriggerChopAttack()
    {
        OnAttackInput(); // Calls your existing attack animation logic
    }
    public void DisableAxeCollider()
    {
        if (axeHitbox != null)
        {
            axeHitbox.enabled = false;
            // No need to reset canRegisterHit here, it's done on EnableAxeCollider
            Debug.Log("Axe collider DISABLED");
        }
    }
    public bool TryRegisterHit()
    {
        if (canRegisterHit)
        {
            canRegisterHit = false; // Disable hit registration immediately
            // Start a short coroutine to re-enable hit registration after a delay
            StartCoroutine(HitCooldownCoroutine());
            return true; // Successfully registered a hit
        }
        return false; // Could not register hit (still on cooldown)
    }

    private IEnumerator HitCooldownCoroutine()
    {
        yield return new WaitForSeconds(hitCooldown);
        canRegisterHit = true; // Allow hit registration again after cooldown
    }



}
