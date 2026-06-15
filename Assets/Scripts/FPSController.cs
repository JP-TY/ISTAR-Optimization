using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 1. Required namespace

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 20f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    public bool canMove = true;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Get pointers to current hardware devices
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        if (keyboard == null || mouse == null) return;

        // 1. Handles Movement Input
        // Read WASD / Arrow Keys
        float moveForward = 0f;
        float moveSide = 0f;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveForward = 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveForward = -1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveSide = 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveSide = -1f;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Left Shift to run
        bool isRunning = keyboard.leftShiftKey.isPressed;
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * moveForward : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * moveSide : 0;
        float movementDirectionY = moveDirection.y;
        
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // 2. Handles Jumping & Gravity
        // Spacebar to jump
        if (keyboard.spaceKey.wasPressedThisFrame && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // 3. Handles Rotation (Mouse)
        if (canMove)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();

            rotationX += -mouseDelta.y * lookSpeed * 0.1f; // Scaled down for New Input delta values
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, mouseDelta.x * lookSpeed * 0.1f, 0);
        }

        // 4. Move Controller
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
