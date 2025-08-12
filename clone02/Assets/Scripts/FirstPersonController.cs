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

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    [Header("Teleport Settings")]
    [SerializeField] private float teleportRange = 15f;
    [SerializeField] private LayerMask teleportMask; 
    [SerializeField] private GameObject teleportMarkerPrefab;
    [SerializeField] private float markerYOffset = 0.05f;

    private Vector3 currentMovement;
    private float verticalRotation;
    private GameObject teleportMarkerInstance;
    private bool hasValidTeleportPoint;
    private Vector3 teleportPoint;
    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier : 1);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (teleportMarkerPrefab != null)
        {
            teleportMarkerInstance = Instantiate(teleportMarkerPrefab);
            teleportMarkerInstance.SetActive(false);
        }
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleTeleportation();
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
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, teleportRange, teleportMask))
            {
                hasValidTeleportPoint = true;
                teleportPoint = hit.point;
                if (teleportMarkerInstance != null)
                {
                    teleportMarkerInstance.transform.position = hit.point + Vector3.up * markerYOffset;
                    teleportMarkerInstance.SetActive(true);
                }
            }
            else
            {
                hasValidTeleportPoint = false;
                if (teleportMarkerInstance != null)
                    teleportMarkerInstance.SetActive(false);
            }
        }
        else
        {
            if (hasValidTeleportPoint)
            {
                characterController.enabled = false;
                transform.position = teleportPoint;
                characterController.enabled = true;
            }

            if (teleportMarkerInstance != null)
                teleportMarkerInstance.SetActive(false);

            hasValidTeleportPoint = false;
        }
    }

}
