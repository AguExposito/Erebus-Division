using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] CinemachineInputAxisController cinemachineInputAxisController;
    [SerializeField] CinemachinePanTilt cinemachinePanTilt;


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
    [SerializeField] float rotationDuration=0.5f;

    [Header("Encounter Restrictions")]
    [SerializeField] float xPanE = 60;
    [SerializeField] bool wrapXE = false;
    [SerializeField] float yTiltE = 70;
    [SerializeField] bool wrapYE = false;

    [Header("Default Restrictions")]
    [SerializeField] float xPan = 180;
    [SerializeField] bool wrapX = true;
    [SerializeField] float yTilt = 70;
    [SerializeField] bool wrapY = false;

    [Space]
    [Header("State Variables")]
    [SerializeField] bool canMove = true;
    [SerializeField] bool isRotatingJumpscare = false;

    [Space]
    [Header("Read Only Variables"), ReadOnly]
    [SerializeField] Vector3 moveDirection = Vector3.zero;
    [SerializeField] float rotationX = 0;
    [SerializeField] CharacterController characterController;
    private Quaternion targetRotation;
    private float timeElapsed;
    private CameraTarget cameraTarget = new CameraTarget();

    void Start()
    {
        List<CinemachineCamera> cinemachines = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.InstanceID).ToList();
        cinemachineCam = GameObject.FindGameObjectWithTag("MainCinemachine").GetComponent<CinemachineCamera>();
        cinemachineInputAxisController = cinemachineCam.GetComponent<CinemachineInputAxisController>();
        cinemachinePanTilt = cinemachineCam.GetComponent<CinemachinePanTilt>();


        foreach (var virtualCamera in cinemachines)
        {
            cameraTarget.TrackingTarget = cameraHolder;
            virtualCamera.Target = cameraTarget;
        }

        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        #region Handles Rotation
        //characterController.Move(moveDirection * Time.deltaTime);

        if (canMove && !isRotatingJumpscare)
        {
            // Rotar el cuerpo del jugador (solo en Y) para que siga a la cámara
            Vector3 lookDirection = cameraTransform.forward;
            lookDirection.y = 0f;
            transform.forward = lookDirection.normalized;
        }

        #endregion

        if (Time.timeScale == 0 || !canMove) return; // If the game is paused, skip the update

        #region Handles Movment & Jumping
        Vector2 input = moveInput.action.ReadValue<Vector2>();
        bool isRunning = runInput.action.IsPressed();
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Move relative to the player's rotation (which follows the mouse)
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        move *= currentSpeed;

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        //// Jump
        //if (characterController.isGrounded)
        //{
        //    moveDirection.y = -1f; // A small value to keep grounded
        //    if (jumpInput.action.WasPressedThisFrame())
        //    {
        //        moveDirection.y = jumpPower;
        //    }
        //}

        Vector3 finalMove = new Vector3(move.x, moveDirection.y, move.z);
        characterController.Move(finalMove * Time.deltaTime);

        #endregion
    }

    public IEnumerator RotateCameraPlayer(Transform targetTransform)
    {
        isRotatingJumpscare = true;
        canMove = false;

        // Deshabilitar el control de entrada
        foreach (var controller in cinemachineInputAxisController.Controllers)
        {
            controller.InputValue = 0;
            controller.Enabled = false;
        }
        cinemachineInputAxisController.enabled = false;

        // Obtener la dirección hacia el objetivo desde la posición del player
        Vector3 directionToTarget = targetTransform.position - transform.position;
        directionToTarget.y = 0; // Mantener solo la dirección horizontal para el player

        // Calcular el ángulo de rotación del player (solo en Y)
        float targetPlayerAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        
        // Obtener la rotación inicial del player
        float initialPlayerAngle = transform.eulerAngles.y;
        
        // Normalizar los ángulos para evitar rotaciones largas
        float angleDifference = Mathf.DeltaAngle(initialPlayerAngle, targetPlayerAngle);
        targetPlayerAngle = initialPlayerAngle + angleDifference;

        timeElapsed = 0f;

        // Rotar el player primero
        while (timeElapsed < rotationDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            float t = timeElapsed / rotationDuration;
            
            // Interpolar la rotación del player
            float currentAngle = Mathf.LerpAngle(initialPlayerAngle, targetPlayerAngle, t);
            transform.rotation = Quaternion.Euler(0, currentAngle, 0);
            
            yield return null;
        }

        // Asegurar que el player esté exactamente en la posición final
        transform.rotation = Quaternion.Euler(0, targetPlayerAngle, 0);

        // Ahora calcular la dirección hacia el objetivo desde la nueva posición del player
        Vector3 newDirectionToTarget = targetTransform.position - transform.position;
        
        // Calcular los ángulos de pan y tilt para la cámara
        float targetPanAngle = Mathf.Atan2(newDirectionToTarget.x, newDirectionToTarget.z) * Mathf.Rad2Deg;
        float targetTiltAngle = Mathf.Asin(newDirectionToTarget.y / newDirectionToTarget.magnitude) * Mathf.Rad2Deg;

        // Obtener los valores iniciales de pan y tilt
        float initialPan = cinemachinePanTilt.PanAxis.Value;
        float initialTilt = cinemachinePanTilt.TiltAxis.Value;

        // Normalizar los ángulos de pan para evitar rotaciones largas
        float panDifference = Mathf.DeltaAngle(initialPan, targetPanAngle);
        targetPanAngle = initialPan + panDifference;

        timeElapsed = 0f;

        // Rotar la cámara
        while (timeElapsed < rotationDuration)
        {
            timeElapsed += Time.unscaledDeltaTime;
            float t = timeElapsed / rotationDuration;
            
            // Interpolar pan y tilt
            float currentPan = Mathf.LerpAngle(initialPan, targetPanAngle, t);
            float currentTilt = Mathf.Lerp(initialTilt, targetTiltAngle, t);
            
            // Actualizar los valores de Cinemachine
            cinemachinePanTilt.PanAxis.Value = currentPan;
            cinemachinePanTilt.TiltAxis.Value = currentTilt;
            
            yield return null;
        }

        // Asegurar que la cámara esté exactamente en la posición final
        cinemachinePanTilt.PanAxis.Value = targetPanAngle;
        cinemachinePanTilt.TiltAxis.Value = targetTiltAngle;

        // Aplicar las restricciones de encuentro
        //ClampPanTilt(xPanE, yTiltE, wrapXE, wrapYE);

        // Sincronizar la rotación del player con la cámara
        Vector3 finalLookDirection = (targetTransform.position - transform.position).normalized;
        finalLookDirection.y = 0;
        transform.forward = finalLookDirection;

        // Esperar un frame para que Cinemachine procese los cambios
        yield return null;

        // Restaurar el control de entrada
        cinemachineInputAxisController.enabled = true;
        foreach (var controller in cinemachineInputAxisController.Controllers)
        {
            controller.InputValue = 0;
            controller.Enabled = true;
        }

        isRotatingJumpscare = false;
        timeElapsed = 0f;
    }
    void ClampPanTilt(float pan, float tilt, bool wrapx = false, bool wrapy = false) {
        float tempPanX = cinemachinePanTilt.PanAxis.Value - pan;
        float tempPanY = cinemachinePanTilt.PanAxis.Value + pan;
        Vector2 tempPan= new Vector2(tempPanX, tempPanY);
        
        float tempTiltX = cinemachinePanTilt.TiltAxis.Value - tilt;
        float tempTiltY = cinemachinePanTilt.TiltAxis.Value + tilt;
        Vector2 tempTilt= new Vector2(tempTiltX, tempTiltY);

        SetPanTilt(tempPan, tempTilt, wrapx, wrapy);
    }
    void SetPanTilt(Vector2 pan, Vector2 tilt, bool wrapx=false, bool wrapy=false) 
    { 
        cinemachinePanTilt.PanAxis.Range = pan;
        //cinemachinePanTilt.TiltAxis.Range = tilt;
        cinemachinePanTilt.PanAxis.Wrap = wrapx;
        cinemachinePanTilt.TiltAxis.Wrap = wrapy;
    }
    public void GiveBackControlToPlayer() { 
        ClampPanTilt(xPan, yTilt, wrapX, wrapY);
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