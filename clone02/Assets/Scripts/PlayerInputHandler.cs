using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string teleport = "Teleport";
    [SerializeField] private string sneak = "Sneak";

    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction teleportAction;
    private InputAction sneakAction;

    private bool sneakPressedThisFrame = false;

    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered { get; private set; }
    public bool TeleportTriggered { get; private set; }

    public bool SneakPressedThisFrame
    {
        get
        {
            if (sneakPressedThisFrame)
            {
                sneakPressedThisFrame = false; 
                return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        InputActionMap map = playerControls.FindActionMap(actionMapName);

        movementAction = map.FindAction(movement);
        rotationAction = map.FindAction(rotation);
        jumpAction = map.FindAction(jump);
        sprintAction = map.FindAction(sprint);
        teleportAction = map.FindAction(teleport);
        sneakAction = map.FindAction(sneak);

        SubscribeInputEvents();
    }

    private void SubscribeInputEvents()
    {
        movementAction.performed += ctx => MovementInput = ctx.ReadValue<Vector2>();
        movementAction.canceled += ctx => MovementInput = Vector2.zero;

        rotationAction.performed += ctx => RotationInput = ctx.ReadValue<Vector2>();
        rotationAction.canceled += ctx => RotationInput = Vector2.zero;

        jumpAction.performed += ctx => JumpTriggered = true;
        jumpAction.canceled += ctx => JumpTriggered = false;

        sprintAction.performed += ctx => SprintTriggered = true;
        sprintAction.canceled += ctx => SprintTriggered = false;

        teleportAction.performed += _ => TeleportTriggered = true;
        teleportAction.canceled += _ => TeleportTriggered = false;

        sneakAction.performed += _ => sneakPressedThisFrame = true;
    }
    public bool UsingController
    {
        get
        {
            return Input.GetJoystickNames().Length > 0;
        }
    }

    private void OnEnable() => playerControls.FindActionMap(actionMapName).Enable();
    private void OnDisable() => playerControls.FindActionMap(actionMapName).Disable();
}
