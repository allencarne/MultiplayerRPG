using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Aimer : MonoBehaviour
{
    PlayerInputHandler inputHandler;
    PlayerInput playerInput;
    private float lastAngle;

    bool isMobile = false;

    private void Awake()
    {
        inputHandler = GetComponentInParent<PlayerInputHandler>();
        playerInput = GetComponentInParent<PlayerInput>();
    }

    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            isMobile = true;
        }
    }

    private void Update()
    {
        if (isMobile)
        {
            RotateOnMobile();
        }
        else
        {
            if (inputHandler != null && playerInput)
            {
                if (playerInput.currentControlScheme == "Gamepad")
                {
                    RotateOnGamePad();
                }

                if (playerInput.currentControlScheme == "Keyboard")
                {
                    RotateOnKeyboard();
                }
            }
        }
    }

    void RotateOnKeyboard()
    {
        // Use the correct camera reference based on the player's index
        Camera currentCamera = Camera.main;

        if (currentCamera != null)
        {
            // Convert the mouse position to world space using the correct camera
            Vector3 mouseWorldPos = currentCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, currentCamera.nearClipPlane));

            // Calculate the direction from the Aimer to the mouse position
            Vector3 directionToMouse = mouseWorldPos - transform.position;

            // Calculate the rotation angle needed to face the mouse position
            float rotZ = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

            // Apply the calculated rotation to the Aimer
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
        }
    }

    void RotateOnGamePad()
    {
        // Get the look input values
        float horizontalLook = inputHandler.LookInput.x;
        float verticalLook = inputHandler.LookInput.y;

        // Check if there is input from the right stick
        if (horizontalLook != 0 || verticalLook != 0)
        {
            // Calculate the rotation angle based on input
            lastAngle = Mathf.Atan2(verticalLook, horizontalLook) * Mathf.Rad2Deg;
        }

        // Apply rotation to the Aimer
        transform.rotation = Quaternion.Euler(0f, 0f, lastAngle);
    }

    void RotateOnMobile()
    {
        // Get the look input values
        float horizontalLook = inputHandler.MoveInput.x;
        float verticalLook = inputHandler.MoveInput.y;

        // Check if there is input from the right stick
        if (horizontalLook != 0 || verticalLook != 0)
        {
            // Calculate the rotation angle based on input
            lastAngle = Mathf.Atan2(verticalLook, horizontalLook) * Mathf.Rad2Deg;
        }

        // Apply rotation to the Aimer
        transform.rotation = Quaternion.Euler(0f, 0f, lastAngle);
    }
}
