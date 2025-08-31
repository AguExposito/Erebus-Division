using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Cinemachine;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public PlayerStats playerStats;

    [Space]
    [Header("Camera References")]
    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform cameraTransform;
    [SerializeField] CinemachineCamera cinemachineCam;
    [SerializeField] CinemachineInputAxisController cinemachineInputAxisController;
    [SerializeField] CinemachinePanTilt cinemachinePanTilt;


    [Space]
    [Header("Inputs")]
    public InputSystem_Actions playerInput;
    [SerializeField] InputActionReference runInput;
    [SerializeField] InputActionReference jumpInput;
    [SerializeField] InputActionReference moveInput;
    [Space]
    [SerializeField] InputActionReference attackInput;
    [SerializeField] InputActionReference satchelInput;
    [SerializeField] InputActionReference dialogueInput;
    [SerializeField] InputActionReference fleeInput;
    [SerializeField] InputActionReference interactInput;
    [SerializeField] InputActionReference goBackInput;

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
    [SerializeField] float rotationDuration=0.1f;

    [Space]
    [Header("State Variables")]
    [SerializeField] bool canMove = true;
    [SerializeField] bool isRotatingJumpscare = false;
    [SerializeField] bool isInCombat = false;
    [SerializeField] public bool isInElevator = false;
    [SerializeField] public bool isInShop = false;
    [SerializeField] public bool isShopHUD = false;
    [SerializeField] bool isAttackHUD = false;
    [SerializeField] bool isSatchelHUD = false;
    [SerializeField] bool isDialogueHUD = false;

    [Space]
    [Header("Read Only Variables"), ReadOnly]
    [SerializeField] Vector3 moveDirection = Vector3.zero;
    [SerializeField] float rotationX = 0;
    [SerializeField] CharacterController characterController;

    private float timeElapsed;
    private CameraTarget cameraTarget = new CameraTarget();
    public bool encounterHUDActive = false;
    void Start()
    {
        playerInput = new InputSystem_Actions();
        playerInput.Enable();
        playerInput.Encounter.Disable();
        playerInput.Player.Enable();

        // Inicializar los InputActionReference del modo Player (modo inicial)
        if (moveInput?.action != null) moveInput.action.Enable();
        if (runInput?.action != null) runInput.action.Enable();
        if (jumpInput?.action != null) jumpInput.action.Enable();

        // Deshabilitar los InputActionReference del modo Encounter
        if (attackInput?.action != null) attackInput.action.Disable();
        if (satchelInput?.action != null) satchelInput.action.Disable();
        if (dialogueInput?.action != null) dialogueInput.action.Disable();
        if (fleeInput?.action != null) fleeInput.action.Disable();
        if (interactInput?.action != null) interactInput.action.Disable();
        if (goBackInput?.action != null) goBackInput.action.Disable();

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

        GameManagerDD.instance.encounterHUD.SetActive(false);
        playerStats.isItsTurn = true;
        EntityLogManager.Instance.GetEntityLog("ManThing").UpdateDialogueNumber();


        if (SceneManager.GetActiveScene().name == "Shop") { isInShop = true; }
        else
        {
            TurnManager.instance.playerStats = playerStats;
            TurnManager.instance.AddTurn(playerStats);
        }

        LoadPlayerContext();
    }

    private void LoadPlayerContext()
    {
        InventoryManager.Instance.UpdateReferences();
        playerStats.baseAttackPower = InventoryManager.Instance.baseAttackPower;
        playerStats.baseCritChance = InventoryManager.Instance.baseCritChance;
        playerStats.baseDodgeChance = InventoryManager.Instance.baseDodgeChance;
        playerStats.baseHitChance = InventoryManager.Instance.baseHitChance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            GameManagerDD.instance.flashlight.SetActive(!GameManagerDD.instance.flashlight.activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AlternatePauseMenu();
        }

        #region Handles HUDs
        if (isInCombat || isInElevator || isInShop)
        {
            RaycastHit[] hits= Physics.RaycastAll(cameraTransform.position, cameraTransform.forward, 10f, LayerMask.GetMask("Enemy"));
            
            foreach (var hit in hits)
            {
                if (hit.transform.CompareTag("AttackingEnemy") && isInCombat)
                {
                    EntityInterface entityInterface=null;

                    if (hit.transform.GetComponentInParent<EntityInterface>() && isInCombat)
                    {
                        hit.transform.GetComponentInParent<EntityInterface>().OnRaycastEnter();
                        entityInterface = hit.transform.GetComponentInParent<EntityInterface>();
                        GameManagerDD.instance.encounterHUD.SetActive(true);
                        encounterHUDActive = true;
                    }
                    else
                    {
                        Debug.LogWarning("EntityInterface not found on the hit object.");
                    }

                    if (isAttackHUD && entityInterface!=null && isInCombat && !isDialogueHUD)
                    {
                        if(hit.collider.TryGetComponent<EnemyPart>(out EnemyPart enemyPart))
                        {
                            entityInterface = hit.transform.GetComponentInParent<EntityInterface>(true);

                            if (enemyPart != null)
                            {
                                playerStats.critChance = (float)Math.Round(enemyPart.critChanceMultiplier * playerStats.baseCritChance, 1);
                                playerStats.hitChance = (float)Math.Round(enemyPart.hitChanceMultiplier * playerStats.baseHitChance * playerStats.GetFearMult(), 1);
                                playerStats.targetEnemyPart = enemyPart;

                                GameManagerDD.instance.critChance.text = playerStats.critChance + "%";
                                GameManagerDD.instance.hitChance.text = playerStats.hitChance + "%";
                                GameManagerDD.instance.bodyPart.text = enemyPart.partType.ToString();
                                GameManagerDD.instance.bodyPartState.text = enemyPart.partStatus.ToString();


                                if (interactInput.action.WasPressedThisFrame() && playerInput.Encounter.enabled && playerStats.isItsTurn && isInCombat && TurnManager.instance.AttackAnimEnded()) 
                                {
                                    playerStats.Attack(entityInterface);
                                    playerInput.Encounter.Disable();
                                    playerInput.Player.Disable();
                                }
                            }
                        }
                    }
                    if (isDialogueHUD && entityInterface != null && isInCombat && !isAttackHUD)
                    {
                        if (hit.collider.TryGetComponent<EnemyPart>(out EnemyPart enemyPart))
                        {
                            entityInterface = hit.transform.GetComponentInParent<EntityInterface>(true);
                            if (interactInput.action.WasPressedThisFrame() && playerInput.Encounter.enabled && playerStats.isItsTurn && isInCombat && TurnManager.instance.AttackAnimEnded())
                            {
                                EntityLogManager.Instance.GetEntityLog(entityInterface.entityName).IncrementDialogueCount(1);
                                EntityLogManager.Instance.GetEntityLog("ManThing").UpdateDialogueNumber();
                                TurnManager.instance.EndTurn(playerStats);
                            }
                        }
                    }
                }

                if (isInElevator && hit.transform.CompareTag("ElevatorButton")) 
                {
                    GameManagerDD.instance.exitHUD.SetActive(true);
                    if (interactInput.action.WasPressedThisFrame())
                    {
                        hit.collider.TryGetComponent<ElevatorButtonController>(out ElevatorButtonController elevatorButton);
                        elevatorButton.onButtonPress.Invoke();
                        hit.transform.gameObject.layer = LayerMask.NameToLayer("Default");
                        canMove = false;
                        GameManagerDD.instance.exitHUD.SetActive(false);
                    }
                }

                Debug.LogWarning(isInShop + "" + hit.transform.tag);

                if (isInShop && hit.transform.CompareTag("Shop")) 
                {
                    Debug.LogWarning("AAAAAAAAAAAAAAAAAAAAAA");
                    if (!interactInput.action.enabled || !playerInput.Player.enabled) 
                    {
                        playerInput.Enable();
                        playerInput.Encounter.Disable();
                        playerInput.Player.Enable();
                        interactInput.action.Enable();
                    }
                    if (interactInput.action.WasPressedThisFrame() && !isShopHUD)
                    {
                        Debug.LogWarning("BBBBBBBBBBBBBBBBBBBBBBBBBBB");
                        OpenShopMenu();
                    }
                }
            }

            if (GameManagerDD.instance.encounterHUD != null && hits.Length==0 && isInCombat)
            {
                GameManagerDD.instance.encounterHUD.SetActive(false);
                encounterHUDActive = false;
            }

            if (attackInput.action.WasPressedThisFrame() && playerInput.Encounter.enabled && isInCombat && !InventoryManager.Instance.scroller.isSatchelOpen && !isDialogueHUD)
            {
                if (!isAttackHUD && encounterHUDActive)
                {
                    isAttackHUD = true;
                    GameManagerDD.instance.enemyInfo.SetActive(true);
                }
                else {
                    isAttackHUD = false;
                    GameManagerDD.instance.enemyInfo.SetActive(false);
                }
            }

            if (fleeInput.action.WasPressedThisFrame() && playerInput.Encounter.enabled && isInCombat && !isAttackHUD && playerStats.isItsTurn && TurnManager.instance.AttackAnimEnded() && !isDialogueHUD) 
            {
                playerStats.Flee();

            }
            if (dialogueInput.action.WasPressedThisFrame() && playerInput.Encounter.enabled && isInCombat && !isAttackHUD && playerStats.isItsTurn && TurnManager.instance.AttackAnimEnded()) 
            {
                if (!isDialogueHUD && encounterHUDActive)
                {
                    isDialogueHUD = true;
                    GameManagerDD.instance.enemyDialogue.SetActive(true);
                    EntityLogManager.Instance.GetEntityLog("ManThing").UpdateDialogueNumber();
                }
                else
                {
                    isDialogueHUD = false;
                    GameManagerDD.instance.enemyDialogue.SetActive(false);
                }
            }

            if (!playerInput.Encounter.enabled && playerStats.isItsTurn && isInCombat)
            {
                playerInput.Encounter.Enable();
            }

            if (playerInput.Encounter.enabled && isInCombat && playerStats.isItsTurn && TurnManager.instance.AttackAnimEnded() && !isAttackHUD && !isDialogueHUD) 
            {
                if (satchelInput.action.WasPressedThisFrame())
                {
                    InventoryManager.Instance.scroller.ToggleSatchel();
                }
                if (interactInput.action.WasPressedThisFrame() && InventoryManager.Instance.scroller.isSatchelOpen) 
                {
                    InventoryManager.Instance.scroller.RemoveSelectedItem();

                }
            }

            if (GameManagerDD.instance.exitHUD.activeInHierarchy && hits.Length==0) GameManagerDD.instance.exitHUD.SetActive(false);

        }
        #endregion

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

    public void AlternatePauseMenu()
    {
        GameManagerDD.instance.pauseMenu.SetActive(!GameManagerDD.instance.pauseMenu.activeInHierarchy);
        canMove = !GameManagerDD.instance.pauseMenu.activeInHierarchy;
        cinemachineInputAxisController.enabled = !GameManagerDD.instance.pauseMenu.activeInHierarchy;
    }
    public void OpenShopMenu ()
    {
        GameManagerDD.instance.shopMenu.SetActive(true);
        canMove = false;
        cinemachineInputAxisController.enabled = false;
        InventoryManager.Instance.scroller.ToggleSatchel();
        isShopHUD = true;
    }
    public void CloseShopMenu ()
    {
        GameManagerDD.instance.shopMenu.SetActive(false);
        canMove = true;
        cinemachineInputAxisController.enabled = true;
        InventoryManager.Instance.scroller.ToggleSatchel();
        isShopHUD = false;
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

        GameManagerDD.instance.encounterHUD.SetActive(true);
        isInCombat = true;
        
        // Deshabilitar el action map de Player
        playerInput.Player.Disable();
        
        // Habilitar el action map de Encounter
        playerInput.Encounter.Enable();
        
        // Deshabilitar los InputActionReference del modo Player
        if (moveInput?.action != null) moveInput.action.Disable();
        if (runInput?.action != null) runInput.action.Disable();
        if (jumpInput?.action != null) jumpInput.action.Disable();
        
        // Habilitar los InputActionReference del modo Encounter
        if (attackInput?.action != null) attackInput.action.Enable();
        if (satchelInput?.action != null) satchelInput.action.Enable();
        if (dialogueInput?.action != null) dialogueInput.action.Enable();
        if (fleeInput?.action != null) fleeInput.action.Enable();
        if (interactInput?.action != null) interactInput.action.Enable();
        if (goBackInput?.action != null) goBackInput.action.Enable();

        isRotatingJumpscare = false;
        timeElapsed = 0f;
    }
    public void GiveBackControlToPlayer()
    {
        isInCombat = false;
        GameManagerDD.instance.encounterHUD.SetActive(false);
        GameManagerDD.instance.enemyInfo.SetActive(false);
        encounterHUDActive = false;
        isAttackHUD = false;
        
        // Deshabilitar el action map de Encounter
        playerInput.Encounter.Disable();
        
        // Habilitar el action map de Player
        playerInput.Player.Enable();
        
        // Habilitar solo los InputActionReference del modo Player
        if (moveInput?.action != null) moveInput.action.Enable();
        if (runInput?.action != null) runInput.action.Enable();
        if (jumpInput?.action != null) jumpInput.action.Enable();
        
        // Deshabilitar los InputActionReference del modo Encounter
        if (attackInput?.action != null) attackInput.action.Disable();
        if (satchelInput?.action != null) satchelInput.action.Disable();
        if (dialogueInput?.action != null) dialogueInput.action.Disable();
        if (fleeInput?.action != null) fleeInput.action.Disable();
        if (interactInput?.action != null) interactInput.action.Disable();
        if (goBackInput?.action != null) goBackInput.action.Disable();
        
        // Restaurar el control de movimiento
        canMove = true;
    }

    


    public void ChangeMovementVariables(float walkSpeed, float runSpeed, float jumpPower, bool alteredMovement) { 
        this.walkSpeed = walkSpeed;
        this.runSpeed = runSpeed;
        this.jumpPower = jumpPower;
        this.alteredMovement = alteredMovement;
    }   

    private void OnEnable()
    {
        // Habilitar inputs según el modo actual
        if (!isInCombat)
        {
            // Modo Player
            if (moveInput?.action != null) moveInput.action.Enable();
            if (runInput?.action != null) runInput.action.Enable();
            if (jumpInput?.action != null) jumpInput.action.Enable();
        }
        else
        {
            // Modo Encounter
            if (attackInput?.action != null) attackInput.action.Enable();
            if (satchelInput?.action != null) satchelInput.action.Enable();
            if (dialogueInput?.action != null) dialogueInput.action.Enable();
            if (fleeInput?.action != null) fleeInput.action.Enable();
            if (interactInput?.action != null) interactInput.action.Enable();
            if (goBackInput?.action != null) goBackInput.action.Enable();
        }
    }
    
    private void OnDisable()
    {
        if (jumpInput?.action != null) jumpInput.action.Disable();
        if (runInput?.action != null) runInput.action.Disable();
        if (moveInput?.action != null) moveInput.action.Disable();
        if (attackInput?.action != null) attackInput.action.Disable();
        if (satchelInput?.action != null) satchelInput.action.Disable();
        if (dialogueInput?.action != null) dialogueInput.action.Disable();
        if (fleeInput?.action != null) fleeInput.action.Disable();
        if (interactInput?.action != null) interactInput.action.Disable();
        if (goBackInput?.action != null) goBackInput.action.Disable();
    }
}