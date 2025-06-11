using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f;

    public float stamina = 100f;
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f; // per second
    public float staminaRegenRate = 25f; // per second (adjustable)
    private float timeSinceStaminaUsed = 0f;
    private bool canRegenStamina = true;

    public bool isClimbing = false;
    public float climbSpeed = 3f;

    public float climbCheckDistance = 1.5f;
    public LayerMask climbableMask;
    public float latchTime = 0.2f; // Delay to prevent instant fall
    private Transform climbTarget; // Optional: store the wall being climbed
    public float climbStaminaDrainRate = 10f; // units per second



    public CharacterController controller;
    public Transform groundCheck;
    public float groundCheckDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 moveDirection;
    private Vector3 velocity;

    public bool isGrounded;
    public bool isCrouching;
    public bool isSprinting;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        GroundCheck();
        HandleSprint();
        Move();
        ApplyGravity();
        HandleJump();
        HandleCrouch();
        RegenerateStamina();
        CheckForClimbable();
        HandleClimb();
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void HandleSprint()
    {
        bool sprintKeyHeld = Input.GetKey(KeyCode.LeftShift);
        bool attemptingToSprint = sprintKeyHeld && !isCrouching && isGrounded && stamina > 0;

        if (attemptingToSprint)
        {
            isSprinting = true;
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            canRegenStamina = false;
            timeSinceStaminaUsed = 0f;
        }
        else
        {
            isSprinting = false;
            if (!canRegenStamina)
            {
                timeSinceStaminaUsed += Time.deltaTime;
                if (timeSinceStaminaUsed >= 1.5f)
                {
                    canRegenStamina = true;
                }
            }
        }
    }

    void RegenerateStamina()
    {
        if (canRegenStamina && stamina < maxStamina && !isClimbing)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }


    void Move()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(inputX, 0f, inputZ).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            // Rotate towards movement direction
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        moveDirection = transform.forward * inputDirection.magnitude;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (isClimbing) return;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // Only latch if also pressing forward input
            if (CheckClimbOnJump() && IsMovingTowardClimbable())
            {
                isClimbing = true;
                velocity = Vector3.zero;
            }
            else
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // regular jump
            }
        }
    }



    void HandleCrouch()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = true;
            Debug.Log("Crouch");
        }
        else if (isGrounded && Input.GetKeyUp(KeyCode.C))
        {
            isCrouching = false;
            Debug.Log("notCrouch");
        }
    }
    
    void CheckForClimbable()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, climbCheckDistance, climbableMask))
        {
            if (Input.GetKey(KeyCode.W))
            {
                isClimbing = true;
            }
        }
        else
        {
            if (isClimbing)
            {
                isClimbing = false;
            }
        }
    }

    void HandleClimb()
    {
        if (!isClimbing) return;

        // Stamina drain while climbing
        stamina -= climbStaminaDrainRate * Time.deltaTime;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        // If out of stamina, fall
        if (stamina <= 0f)
        {
            CancelClimb();
            return;
        }

        // Movement
        float vertical = Input.GetAxis("Vertical");
        Vector3 climbMovement = new Vector3(0, vertical, 0);
        controller.Move(climbMovement * climbSpeed * Time.deltaTime);

        // Climb-top check
        if (CheckIfAtClimbTop() && vertical > 0f)
        {
            FinishClimb();
        }

        // Jump off manually
        if (Input.GetButtonDown("Jump"))
        {
            CancelClimb(jumpAway: true);
        }
    }



    
    bool CheckClimbOnJump()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1f;
        if (Physics.Raycast(origin, transform.forward, out hit, climbCheckDistance, climbableMask))
        {
            climbTarget = hit.transform;
            return true;
        }
        return false;
    }
    
    bool IsMovingTowardClimbable()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 worldInput = transform.TransformDirection(input).normalized;

        // Check if movement input is roughly aligned with forward
        float dot = Vector3.Dot(worldInput, transform.forward);

        return dot > 0.5f; // > 0.5 means fairly forward input
    }

    bool CheckIfAtClimbTop()
    {
        // Cast upward from player to detect ledge clearance
        Vector3 origin = transform.position + Vector3.up * controller.height;
        Ray ray = new Ray(origin, transform.forward);
        float forwardClearanceDistance = 0.5f;

        // No wall in front at head height = we've reached the top
        return !Physics.Raycast(ray, forwardClearanceDistance, climbableMask);
    }

    void FinishClimb()
    {
        if (climbTarget == null) return;

        Vector3 edgeOffset = climbTarget.forward * 0.5f + Vector3.up * 1.5f;
        Vector3 targetPos = climbTarget.position + edgeOffset;

        controller.enabled = false; // Required before modifying position
        transform.position = targetPos;
        controller.enabled = true;

        isClimbing = false;
        climbTarget = null;
    }
    
    void CancelClimb(bool jumpAway = false)
    {
        isClimbing = false;
        climbTarget = null;
        canRegenStamina = false;
        timeSinceStaminaUsed = 0f;
        
        if (!canRegenStamina)
        {
            timeSinceStaminaUsed += Time.deltaTime;
            if (timeSinceStaminaUsed >= 1.5f)
            {
                canRegenStamina = true;
            }
        }
        
        if (jumpAway)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }



}
