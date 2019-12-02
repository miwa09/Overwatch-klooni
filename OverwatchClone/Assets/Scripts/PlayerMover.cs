﻿using System.Collections;
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

    void Start()
    {
        rig = gameObject.GetComponent<Rigidbody>();
        //groundCull = 1 << groundMask.value;
        //groundCull = ~groundCull;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded && !jumped) {
            rig.useGravity = false;
            rig.velocity = Vector3.zero;
        } else rig.useGravity = true;
        if (Input.GetButtonDown(inputPrefix + "Jump") && isGrounded) {
            jumped = true;
        }
        if (jumped) {
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
        if (targetVelocity.magnitude > 0.1f) {
            lastTargetVelocity = targetVelocity;
        }

        if (Physics.Raycast(groundCheck.position, targetVelocity, out groundHit, Vector3.Distance(groundCheck.position, targetVelocity) * Time.fixedDeltaTime, groundMask)) {
            Vector3 location = groundHit.point;
            rig.position += new Vector3(0, location.y - groundCheck.position.y, 0);
        }
        if (Physics.Raycast(frontCheck.position, targetVelocity, out frontHit, Vector3.Distance(frontCheck.position, targetVelocity) * Time.fixedDeltaTime, groundMask)) {
            Vector3 location = frontHit.point;
            if (!stopped) {
                rig.position = location - targetVelocity.normalized * 0.6f;
            }
            hasHitWall = true;
            stopped = true;
            if (stopped) {
                if (!Physics.Raycast(frontCheck.position + lastTargetVelocity.normalized * 0.6f, lastTargetVelocity, out frontHit, Vector3.Distance(frontCheck.position, lastTargetVelocity) * Time.fixedDeltaTime)) {
                    stopped = false;
                    
                    print("not stopped");
                }
            }
        }


        Debug.DrawRay(groundCheck.position, targetVelocity * Time.fixedDeltaTime, Color.red);
        if (!hasHitWall) {
            Debug.DrawRay(frontCheck.position + inputVelocity * 0.6f, targetVelocity * Time.fixedDeltaTime, Color.blue);
        }
        Debug.DrawRay(frontCheck.position + Vector3.up + lastTargetVelocity.normalized * 0.6f, lastTargetVelocity * Time.fixedDeltaTime, Color.yellow);

        if (jumped && isGrounded) {
            rig.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y), ForceMode.VelocityChange);
        }

            Vector3 velocity = rig.velocity;
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            if (!stopped && !hasHitWall) {
            rig.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    public void moverSprint(float speed) {
        movementSpeed = speed;
    }

}
