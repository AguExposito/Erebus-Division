using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform cameraTransform;
    [SerializeField] CinemachineCamera cinemachineCam;


    [Space]
    [Header("Inputs")]
    [SerializeField] InputActionReference runInput;
    [SerializeField] InputActionReference jumpInput;
    [SerializeField] InputActionReference moveInput;

    [Space]
    [Header("Movement Variables")]
    [SerializeField] public float walkSpeed;
    [SerializeField] public float runSpeed;
    [SerializeField] public float jumpPower;
    [SerializeField] float gravity;
    [SerializeField] public bool alteredMovement;

    [Space]
    [Header("Camera Variables")]
    [SerializeField] public float lookSpeed;
    [SerializeField] float lookXLimit;

    [Space]
    [Header("State Variables")]
    [SerializeField] bool canMove = true;

    [Space]
    [Header("Read Only Variables"), ReadOnly]
    [SerializeField] Vector3 moveDirection = Vector3.zero;
    [SerializeField] float rotationX = 0;
    [SerializeField] CharacterController characterController;

    void Start()
    {
        cinemachineCam = GameObject.FindGameObjectWithTag("Cinemachine").GetComponent<CinemachineCamera>();
        CameraTarget cameraTarget = new CameraTarget();
        cameraTarget.TrackingTarget = cameraHolder;
        cinemachineCam.Target= cameraTarget;

        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        #region Handles Movment & Jumping
        Vector2 input = moveInput.action.ReadValue<Vector2>();
        bool isRunning = runInput.action.IsPressed();
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Move relative to the player's rotation (which follows the mouse)
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        move *= currentSpeed;

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Jump
        if (characterController.isGrounded)
        {
            moveDirection.y = -1f; // A small value to keep grounded
            if (jumpInput.action.WasPressedThisFrame())
            {
                moveDirection.y = jumpPower;
            }
        }

        Vector3 finalMove = new Vector3(move.x, moveDirection.y, move.z);
        characterController.Move(finalMove * Time.deltaTime);

        #endregion

        #region Handles Rotation
        //characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            // Rotar el cuerpo del jugador (solo en Y) para que siga a la cámara
            Vector3 lookDirection = cameraTransform.forward;
            lookDirection.y = 0f;
            transform.forward = lookDirection.normalized;
        }

        #endregion


    }
    public void ChangeMovementVariables(float walkSpeed, float runSpeed, float jumpPower, bool alteredMovement) { 
        this.walkSpeed = walkSpeed;
        this.runSpeed = runSpeed;
        this.jumpPower = jumpPower;
        this.alteredMovement = alteredMovement;
    }
    private void OnEnable()
    {
        jumpInput.action.Enable();
        runInput.action.Enable();
        moveInput.action.Enable();
    }
    private void OnDisable()
    {
        jumpInput.action.Disable();
        runInput.action.Disable();
        moveInput.action.Disable();
    }
}