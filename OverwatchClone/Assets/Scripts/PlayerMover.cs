using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public string inputPrefix = "P1";
    Rigidbody rig;
    public float movementSpeed = 12;
    public float jumpHeight = 3;
    public Transform groundCheck;
    public float groundDistance;
    bool isGrounded;
    public LayerMask groundMask;

    void Start()
    {
        rig = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);  //Checks if the player is on the ground.
        if (isGrounded) {
            rig.useGravity = false;
        }
        float x = Input.GetAxis(inputPrefix + "Horizontal"); //Gets the horizontal and vertical inputs from the controller or keyboard
        float z = Input.GetAxis(inputPrefix + "Vertical");
        Vector3 move = transform.right * x + transform.forward * z; //Calculates where the player is moving
        rig.velocity += move.normalized * movementSpeed * Time.deltaTime;

    }
}
