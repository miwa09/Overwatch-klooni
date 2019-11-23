using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public string inputPrefix = "P1"; //What player is being controller
    public bool isControllableByMouse = false;
    public float mouseSensitivity = 100f;
    float mouseX;
    float mouseY;
    public Transform playerBody;
    float xRotation = 0f;
    float xRotationController = 0f;
    public Transform standing;
    public Transform crouching;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Lock the cursor (Makes it invisible, and so it can't escape the game window)
    }
    void Update()
    {
        if (isControllableByMouse)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; //Get the mouse and joystick X and Y axis'
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }
        float joyStickX = Input.GetAxis(inputPrefix + "JoystickLookX") * mouseSensitivity * Time.deltaTime;
        float joyStickY = Input.GetAxis(inputPrefix + "JoystickLookY") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90); //Don't let the player do a 360 in a Y axis, so they can't look behind them
        xRotationController += joyStickY;
        xRotationController = Mathf.Clamp(xRotationController, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation + xRotationController, 0, 0); //Rotate the player based on input
        playerBody.Rotate(Vector3.up * (mouseX + joyStickX));
    }

    public void CameraCrouched()
    {
        transform.position = Vector3.MoveTowards(transform.position, crouching.position, 0.04f);
    }
    public void CameraStanding()
    {
        transform.position = Vector3.MoveTowards(transform.position, standing.position, 0.07f);
    }
}
