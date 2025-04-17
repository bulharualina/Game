using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float blendSpeed = 0.02f;

    private PlayerInput playerInput;

    private static int inputXHash = Animator.StringToHash("inputX");
    private static int inputYHash = Animator.StringToHash("inputY");

    private Vector3 currentBlendInput = Vector3.zero;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        Vector2 inputTarget = playerInput.MovementInput;
        currentBlendInput = Vector3.Lerp(currentBlendInput, inputTarget, blendSpeed * Time.deltaTime);

        animator.SetFloat(inputXHash, inputTarget.x);
        animator.SetFloat (inputYHash, inputTarget.y);
    }
}
