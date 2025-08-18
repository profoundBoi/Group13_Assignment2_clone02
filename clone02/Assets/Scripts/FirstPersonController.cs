using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.005f;
    [SerializeField] private float upDownLookRange = 80f;
    [SerializeField] private float controllerSensitivity = 2.0f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private Animator playerAnimator;

    [Header("Teleport Settings")]
    [SerializeField] private float teleportRange = 15f;
    [SerializeField] private LayerMask teleportMask; 
    [SerializeField] private GameObject teleportMarkerPrefab;
    [SerializeField] private float markerYOffset = 0.05f;
   

    [Header("Teleport Visuals")]
    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float pulseScale = 0.2f;

    [Header("Aim Settings")]
    [SerializeField] private float aimTimeScale = 0.3f;
    [SerializeField] private float zoomFOV = 40f;
    [SerializeField] private float zoomSpeed = 10f;

    [Header("Visual Effects")]
    [SerializeField] private CanvasGroup blinkOverlay; 
    [SerializeField] private float overlayFadeSpeed = 5f;
    [SerializeField] private float overlayMaxAlpha = 0.4f;

    [Header("Sneak Settings")]
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float crouchCameraOffset = -0.5f;
    [SerializeField] private KeyCode crouchKey = KeyCode.Tab;
    [SerializeField] private RawImage walkIcon;
    [SerializeField] private RawImage sneakIcon;

    [Header("Teleport Stamina")]
    [SerializeField] private int maxTeleportCharges = 3;
    [SerializeField] private float rechargeTime = 5f;
    [SerializeField] private Slider staminaSlider;

    private Vector3 currentMovement;
    private float verticalRotation;
    private GameObject teleportMarkerInstance;
    private bool hasValidTeleportPoint;
    private Vector3 teleportPoint;
    private float defaultFOV;
    private bool isCrouched = false;
    private float originalCameraY;
    private int currentCharges;
    private float currentStamina;
    private float rechargeTimer;
    private bool isBlinking => playerInputHandler.TeleportTriggered;
    private float currentOverlayAlpha = 0f;

    private float CurrentSpeed
    {
        get
        {
            if (isCrouched)
                return walkSpeed * crouchSpeedMultiplier;
            return walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : 1);
        }
    }

    public bool IsSneaking => isCrouched;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultFOV = mainCamera.fieldOfView;
        originalCameraY = cameraTransform.localPosition.y;

        currentCharges = maxTeleportCharges;
        currentStamina = maxTeleportCharges;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxTeleportCharges;
            staminaSlider.value = currentStamina;
            staminaSlider.interactable = false;
        }

        if (teleportMarkerPrefab != null)
        {
            teleportMarkerInstance = Instantiate(teleportMarkerPrefab);
            teleportMarkerInstance.SetActive(false);
        }

        if (blinkOverlay != null)
        {
            blinkOverlay.alpha = 0f;
            blinkOverlay.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        HandleCrouchToggle();
        HandleMovement();
        HandleRotation();
        HandleTeleportation();
        UpdateMarkerPulse();
        UpdateSneakUI();
        UpdateBlinkOverlay();
    }

    private void UpdateBlinkOverlay()
    {
        if (blinkOverlay == null) return;

        float targetAlpha = isBlinking ? overlayMaxAlpha : 0f;
        currentOverlayAlpha = Mathf.MoveTowards(currentOverlayAlpha, targetAlpha, overlayFadeSpeed * Time.unscaledDeltaTime);

        blinkOverlay.alpha = currentOverlayAlpha;
        blinkOverlay.gameObject.SetActive(currentOverlayAlpha > 0f);
    }

    private Vector3 CalculateWorldDirection()
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }

    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
            if (playerInputHandler.JumpTriggered)
                currentMovement.y = jumpForce;
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        HandleJumping();
        characterController.Move(currentMovement * Time.deltaTime);
    }

    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation()
    {
        float horizontalRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float verticalRotationDelta = playerInputHandler.RotationInput.y * mouseSensitivity;

        ApplyHorizontalRotation(horizontalRotation);
        ApplyVerticalRotation(verticalRotationDelta);
    }

    private void HandleTeleportation()
    {
        if (playerInputHandler.TeleportTriggered && currentCharges > 0)
        {
            Time.timeScale = aimTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomFOV, Time.unscaledDeltaTime * zoomSpeed);

            FindTeleportPoint();

            if (hasValidTeleportPoint && teleportMarkerInstance != null)
            {
                teleportMarkerInstance.transform.position = teleportPoint + Vector3.up * 0.05f;
                teleportMarkerInstance.SetActive(true);
            }
            else if (teleportMarkerInstance != null)
            {
                teleportMarkerInstance.SetActive(false);
            }
        }
        else if (!playerInputHandler.TeleportTriggered)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultFOV, Time.unscaledDeltaTime * 10f);

            if (hasValidTeleportPoint && currentCharges > 0)
            {
                RaycastHit groundHit; 
                Vector3 safeTeleportPoint = teleportPoint;
                if (Physics.Raycast(teleportPoint + Vector3.up, Vector3.down, out groundHit, 5f, teleportMask))
                {
                    safeTeleportPoint = groundHit.point; 
                }

                if (playerAnimator != null)
                {
                    playerAnimator.SetTrigger("Teleport");
                }

                characterController.enabled = false;
                transform.position = safeTeleportPoint + Vector3.up * 0.1f;
                characterController.enabled = true;

                characterController.enabled = false;
                transform.position = safeTeleportPoint + Vector3.up * 0.1f; 
                characterController.enabled = true;

                currentMovement = Vector3.zero;

                currentCharges--;
                currentStamina = currentCharges;
                rechargeTimer = 0f;
                UpdateStaminaUI();
            }

            if (teleportMarkerInstance != null)
                teleportMarkerInstance.SetActive(false);

            hasValidTeleportPoint = false;
        }

        HandleTeleportRecharge();
    }


    private void FindTeleportPoint()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, teleportRange, teleportMask))
        {
            hasValidTeleportPoint = true;
            teleportPoint = hit.point;
        }
        else
        {
            Vector3 arcStart = mainCamera.transform.position + mainCamera.transform.forward * (teleportRange * 0.8f) + Vector3.up * 2f;
            if (Physics.Raycast(arcStart, Vector3.down, out hit, 5f, teleportMask))
            {
                hasValidTeleportPoint = true;
                teleportPoint = hit.point;
            }
            else
            {
                hasValidTeleportPoint = false;
            }
        }
    }

    private void UpdateMarkerPulse()
    {
        if (teleportMarkerInstance != null && teleportMarkerInstance.activeSelf)
        {
            float scale = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseScale;
            teleportMarkerInstance.transform.localScale = Vector3.one * scale;
        }
    }

    private void UpdateSneakUI()
    {
        if (walkIcon != null) walkIcon.enabled = !IsSneaking;
        if (sneakIcon != null) sneakIcon.enabled = IsSneaking;
    }

    private void HandleCrouchToggle()
    {
        if (playerInputHandler.SneakPressedThisFrame)
        {
            isCrouched = !isCrouched;

            if (isCrouched)
            {
                characterController.height = crouchHeight;
                cameraTransform.localPosition = new Vector3(
                    cameraTransform.localPosition.x,
                    originalCameraY + crouchCameraOffset,
                    cameraTransform.localPosition.z
                );
            }
            else
            {
                RaycastHit hit;
                float headClearance = standingHeight - crouchHeight;
                if (!Physics.Raycast(transform.position, Vector3.up, out hit, headClearance))
                {
                    characterController.height = standingHeight;
                    cameraTransform.localPosition = new Vector3(
                        cameraTransform.localPosition.x,
                        originalCameraY,
                        cameraTransform.localPosition.z
                    );
                }
                else
                {
                    isCrouched = true;
                }
            }
        }
    }

    private void HandleTeleportRecharge()
    {
        if (currentCharges < maxTeleportCharges)
        {
            rechargeTimer += Time.deltaTime;

            float fraction = rechargeTimer / rechargeTime;
            currentStamina = currentCharges + Mathf.Clamp01(fraction);

            if (rechargeTimer >= rechargeTime)
            {
                currentCharges++;
                rechargeTimer = 0f;
                currentStamina = currentCharges;
            }

            UpdateStaminaUI();
        }
    }

    private void UpdateStaminaUI()
    {
        if (staminaSlider != null)
            staminaSlider.value = currentStamina;
    }

}
