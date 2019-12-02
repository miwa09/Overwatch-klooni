using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public string inputPrefix = "P1";
    Rigidbody rig;
    public float movementSpeed = 12;
    public float jumpHeight = 3;
    public float maxVelocityChange = 10;
    public Transform groundCheck;
    public float groundDistance;
    public float maxAngle = 90f;
    bool isGrounded;
    public LayerMask groundMask;
    Vector3 move = Vector3.zero;
    public MouseLook playerCamera;
    bool jumped;
    RaycastHit hit;
    bool hitWall;
    float playerRaiser;

    void Start()
    {
        rig = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);  //Checks if the player is on the ground.
        if (isGrounded) {
            rig.useGravity = false;
            rig.velocity = Vector3.zero;
            jumped = false;
            if (Physics.Raycast(groundCheck.position + new Vector3(0,0.1f,0),groundCheck.forward, out hit, 0.4f, groundMask)) {
                Vector3 location = hit.point;
                rig.position += new Vector3(0, location.y - groundCheck.position.y, 0);

            }
        } else rig.useGravity = true;

        Debug.DrawRay(groundCheck.position + new Vector3(0.1f,0), groundCheck.forward * 0.5f, Color.red);
    }
    private void FixedUpdate() {
        //rig.MovePosition(rig.position + move * movementSpeed * Time.fixedDeltaTime);
            Vector3 inputVelocity = new Vector3(Input.GetAxis(inputPrefix + "Horizontal"), 0, Input.GetAxis(inputPrefix + "Vertical"));
            inputVelocity = transform.TransformDirection(inputVelocity);

        if (Input.GetButtonDown(inputPrefix + "Jump") && isGrounded) {
            rig.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y), ForceMode.VelocityChange);
            jumped = true;
        }

        Vector3 targetVelocity = rig.velocity + inputVelocity * movementSpeed;
            Vector3 velocity = targetVelocity * Time.fixedDeltaTime;
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            rig.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}
