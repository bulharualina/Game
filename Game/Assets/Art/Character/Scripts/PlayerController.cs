using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;

    public float runSpeed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private PlayerInput playerInput;

    Vector3 velocity;

    bool isGrounded;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
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
}
