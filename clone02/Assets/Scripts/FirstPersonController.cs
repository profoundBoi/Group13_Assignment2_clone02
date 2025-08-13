using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;
    [SerializeField] private float controllerSensitivity = 2.0f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;

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

    private Vector3 currentMovement;
    private float verticalRotation;
    private GameObject teleportMarkerInstance;
    private bool hasValidTeleportPoint;
    private Vector3 teleportPoint;
    private float defaultFOV;

    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : 1);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultFOV = mainCamera.fieldOfView;

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
        if (blinkOverlay != null && !playerInputHandler.TeleportTriggered)
        {
            blinkOverlay.alpha = 0f;
            blinkOverlay.gameObject.SetActive(false);
        }

        HandleMovement();
        HandleRotation();
        HandleTeleportation();
        UpdateMarkerPulse();
        UpdateOverlayFade();
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
            {
                currentMovement.y = jumpForce;
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            if (blinkOverlay != null)
            {
                blinkOverlay.alpha = Mathf.Lerp(blinkOverlay.alpha, 0f, Time.unscaledDeltaTime * overlayFadeSpeed);

                if (blinkOverlay.alpha < 0.01f)
                    blinkOverlay.gameObject.SetActive(false);
            }
        }

        if (blinkOverlay != null)
        {
            blinkOverlay.gameObject.SetActive(true);
            blinkOverlay.alpha = Mathf.Lerp(blinkOverlay.alpha, overlayMaxAlpha, Time.unscaledDeltaTime * overlayFadeSpeed);
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
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;


        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }

    private void HandleTeleportation()
    {
        if (playerInputHandler.TeleportTriggered)
        {
            Time.timeScale = aimTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, zoomFOV, Time.unscaledDeltaTime * zoomSpeed);

            FindTeleportPoint();

            if (hasValidTeleportPoint && teleportMarkerInstance != null)
            {
                teleportMarkerInstance.transform.position = teleportPoint + Vector3.up * markerYOffset;
                teleportMarkerInstance.SetActive(true);
            }
            else if (teleportMarkerInstance != null)
            {
                teleportMarkerInstance.SetActive(false);
            }

            if (blinkOverlay != null)
            {
                blinkOverlay.gameObject.SetActive(true);
                blinkOverlay.alpha = Mathf.Lerp(blinkOverlay.alpha, overlayMaxAlpha, Time.unscaledDeltaTime * overlayFadeSpeed);
            }
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, defaultFOV, Time.unscaledDeltaTime * zoomSpeed);

            if (hasValidTeleportPoint)
            {
                characterController.enabled = false;
                transform.position = teleportPoint;
                characterController.enabled = true;
            }

            if (teleportMarkerInstance != null)
                teleportMarkerInstance.SetActive(false);

            hasValidTeleportPoint = false;

            if (blinkOverlay != null)
            {
                blinkOverlay.alpha = Mathf.Lerp(blinkOverlay.alpha, 0f, Time.unscaledDeltaTime * overlayFadeSpeed);

                if (blinkOverlay.alpha < 0.01f)
                    blinkOverlay.gameObject.SetActive(false);
            }
        }
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

    private void UpdateOverlayFade()
    {
    }

}
