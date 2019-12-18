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
    public bool isGrounded;
    public LayerMask groundMask;
    int groundCull;
    Vector3 move = Vector3.zero;
    public MouseLook playerCamera;
    bool jumped;
    RaycastHit groundHit;
    RaycastHit frontHit;
    bool hitWall;
    float playerRaiser;
    public Transform frontCheck;
    bool hasHitWall = false;
    bool stopped = false;
    float jumpTimer = 0;
    float jumpTicker = 0.2f;
    Vector3 lastTargetVelocity;
    bool inGround = false;
    int lastFSIndex;
    public float footstepInterval = 0.1f;
    Vector3 lastFootstepSpot;

    void Start()
    {
        rig = gameObject.GetComponent<Rigidbody>();
        lastFootstepSpot = transform.position;
        //groundCull = 1 << groundMask.value;
        //groundCull = ~groundCull;
    }

    // Update is called once per frame
    void Update()
    {

        if (isGrounded && !jumped) {
            rig.useGravity = false;
            rig.velocity = new Vector3 (rig.velocity.x, 0, rig.velocity.z);
        } else rig.useGravity = true;
        if (Input.GetButton(inputPrefix + "Jump") && isGrounded && !jumped) {
            jumped = true;
            rig.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y), ForceMode.VelocityChange);
        }
        if (jumped) {
            //rig.useGravity = true;
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= jumpTicker) {
                jumped = false;
                jumpTimer = 0;
            }
        }
    }
    private void FixedUpdate() {
        //rig.MovePosition(rig.position + move * movementSpeed * Time.fixedDeltaTime);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);  //Checks if the player is on the ground.
            Vector3 inputVelocity = new Vector3(Input.GetAxis(inputPrefix + "Horizontal"), 0, Input.GetAxis(inputPrefix + "Vertical"));
            inputVelocity = transform.TransformDirection(inputVelocity);
            Vector3 targetVelocity = inputVelocity * movementSpeed;
        if (inputVelocity.magnitude > 0 && isGrounded) {
            if (Vector3.Distance(transform.position, lastFootstepSpot) > footstepInterval) {
                PlayFootstep();
                lastFootstepSpot = transform.position;
            }
        }
        if (targetVelocity.magnitude > 3f) {
            lastTargetVelocity = targetVelocity;
        }

        if (Physics.Raycast(groundCheck.position, targetVelocity, out groundHit, Vector3.Distance(groundCheck.position, targetVelocity) * Time.fixedDeltaTime, groundMask) && isGrounded && !jumped) {
            if (Physics.Raycast(groundCheck.position, Vector3.up, 0.01f, groundMask)){
                inGround = true;
            }
            Vector3 location = groundHit.point;
            rig.position += new Vector3(0, location.y - groundCheck.position.y, 0);
        }
        while (inGround) {
            rig.position += Vector3.up * 0.01f;
            if (Physics.Raycast(groundCheck.position, Vector3.up, 0.01f, groundMask)) {
                inGround = true;
            } else inGround = false;
        }
        if (Physics.Raycast(frontCheck.position + targetVelocity.normalized * 0.3f, targetVelocity, out frontHit, Vector3.Distance(frontCheck.position, targetVelocity) * Time.fixedDeltaTime, groundMask) && !hasHitWall) {
            Vector3 location = frontHit.point;
            rig.position = location - targetVelocity.normalized * 0.6f;
            hasHitWall = true;
        }
        if (!Physics.Raycast(frontCheck.position + lastTargetVelocity.normalized * 0.3f, lastTargetVelocity, out frontHit, Vector3.Distance(frontCheck.position, targetVelocity) * Time.fixedDeltaTime, groundMask) && hasHitWall) {
            hasHitWall = false;
        }


        Debug.DrawRay(groundCheck.position, targetVelocity * Time.fixedDeltaTime, Color.red);
        if (!hasHitWall) {
            Debug.DrawRay(frontCheck.position + inputVelocity * 0.6f, targetVelocity * Time.fixedDeltaTime, Color.blue);
        }
        Debug.DrawRay(frontCheck.position + Vector3.up + lastTargetVelocity.normalized * 0.6f, lastTargetVelocity * Time.fixedDeltaTime, Color.yellow);

        //if (jumped && isGrounded) {
        //    rig.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y), ForceMode.VelocityChange);
        //}

            Vector3 velocity = rig.velocity;
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            if (!hasHitWall) {
            rig.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    public void moverSprint(float speed) {
        movementSpeed = speed;
    }

    void PlayFootstep() {
        int FSIndex = Random.Range(1, 8);
        while (FSIndex == lastFSIndex) {
            FSIndex = Random.Range(1, 8);
        }
        AudioFW.Play("footstep" + FSIndex);
    }

}
