using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Movement : MonoBehaviour
{
    Animator animator;

    public float moveSpeed = 1f;

    Vector3 stopPosition;

    float walkTime;
    public float walkCounter;
    float waitTime;
    public float waitCounter;

    int WalkDirection;

    public bool isWalking;

    [Header("Ground Check")]
    public Transform groundCheck; // Assign an empty GameObject as the ground check origin
    public float groundDistance = 1f; // Radius of the ground check sphere
    public LayerMask groundMask; // Layer(s) that are considered ground

    // NEW: Vertical velocity for gravity
    private float verticalVelocity;
    public float gravity = -9.81f * 2; // Match your player's gravity for consistency, or adjust

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        //So that all the prefabs don't move/stop at the same time
        walkTime = Random.Range(10, 15);
        waitTime = Random.Range(3, 5);


        waitCounter = waitTime;
        walkCounter = walkTime;

        ChooseDirection();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyGravity();
        if (isWalking)
        {

            animator.SetBool("isRunning", true);

            walkCounter -= Time.deltaTime;

            switch (WalkDirection)
            {
                case 0:
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 1:
                    transform.localRotation = Quaternion.Euler(0f, 90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 2:
                    transform.localRotation = Quaternion.Euler(0f, -90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 3:
                    transform.localRotation = Quaternion.Euler(0f, 180, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
            }

            if (walkCounter <= 0)
            {
                stopPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                isWalking = false;
                //stop movement
                transform.position = stopPosition;
                animator.SetBool("isRunning", false);
                //reset the waitCounter
                waitCounter = waitTime;
            }


        }
        else
        {

            waitCounter -= Time.deltaTime;

            if (waitCounter <= 0)
            {
                ChooseDirection();
            }
        }
    }

   
    private bool IsGrounded()
    {
       
        if (groundCheck == null)
        {
            Debug.LogError("AI_Movement: GroundCheck Transform is not assigned!");
            return false;
        }
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    
    private void ApplyGravity()
    {
        if (IsGrounded() && verticalVelocity < 0f)
        {
            verticalVelocity = -2f; 
        }

        verticalVelocity += gravity * Time.deltaTime;
    }
    public void ChooseDirection()
    {
        WalkDirection = Random.Range(0, 4);

        isWalking = true;
        walkCounter = walkTime;
    }
}
