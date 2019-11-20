using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public string inputPrefix = "P1"; //What player is being controller

    public CharacterController controller;
    public float movementSpeed = 12f;
    public float crouchMoveSpeed = 6f;
    Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    bool isGrounded;
    bool isCrouched = false;
    float normalMoveSpeed;

    public MouseLook playerCamera;

    private void Start()
    {
        normalMoveSpeed = movementSpeed;
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);  //Checks if the player is on the ground.

        if (isGrounded && velocity.y < 0) //Resets the falling speed when the player is on the ground
        {
            velocity.y = -1f;
        }

        float x = Input.GetAxis(inputPrefix + "Horizontal"); //Gets the horizontal and vertical inputs from the controller or keyboard
        float z = Input.GetAxis(inputPrefix + "Vertical");

        Vector3 move = transform.right * x + transform.forward * z; //Calculates where the player is moving

        controller.Move(move * movementSpeed * Time.deltaTime);  //Moves the player

        if (Input.GetButtonDown(inputPrefix + "Jump") && isGrounded) //It's a jump, go figure
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime; //Calculates falling velocity when players are not touching the ground.

        controller.Move(velocity * Time.deltaTime); //Actually makes the player fall when they're not touching the ground.

        if (Input.GetButton(inputPrefix + "Crouch")) //Crouch input
        {
            isCrouched = true;
        }
        else isCrouched = false; //The player isn't crouched whenever the button isn't being pressed
        if (isCrouched)
        {
            Crouch();
        }
        if (!isCrouched)  //When the player isn't crouched, bring the movement speed back up and tell the camera to stand up
        {
            playerCamera.CameraStanding();
            movementSpeed = normalMoveSpeed;
        }
    }

    void Crouch()
    {
        //print("Crouching!"); //For testing purposes
        movementSpeed = crouchMoveSpeed; //Slow the player's movement speed when crouched
        playerCamera.CameraCrouched();  //Tell the player camera to crouch
    }
}
