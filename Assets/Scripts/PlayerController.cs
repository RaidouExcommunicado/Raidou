using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public CharacterController controller;

    private Vector3 moveDirection;
    private Vector3 velocity;

    public bool isCrouching;
    public bool isGrounded;
    public Transform groundCheck;
    public float groundCheckDistance;
    public LayerMask groundMask;

    public float gravity = -9.81f;
    public float jumpHeight;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }

        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        //Move
        moveDirection = new Vector3(inputX * moveSpeed, 0f, inputZ * moveSpeed);
        moveDirection = transform.TransformDirection(moveDirection);

        controller.Move(moveDirection * Time.deltaTime);

        //Jump
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

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

}
