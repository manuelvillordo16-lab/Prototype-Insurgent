using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CharacterController controller;

    [Header("Velocidades")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float crouchSpeed = 2.5f;

    [Header("Salto y gravedad")]
    [SerializeField] private float jumpHeight = 1.6f;
    [SerializeField] private float gravity = -15f;

    [Header("Agacharse")]
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float crouchingHeight = 1.2f;
    [SerializeField] private float heightChangeSpeed = 10f;
    [SerializeField] private Transform visualTransform;

    [Header("Interacción")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("Cámara")]
    [SerializeField] private Transform lookTarget; //LookTarget Object
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = -40f;
    [SerializeField] private float maxVerticalAngle = 80f;

    private float currentYaw;
    private float currentPitch;

    // Input
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isSprinting;
    private bool isCrouching;

    // Physics
    private Vector3 velocity;
    private float currentHeight;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        currentHeight = standingHeight;
        controller.height = standingHeight;

        if (lookTarget == null)
            lookTarget = transform.Find("LookTarget");
    }

    private void Update()
    {
        HandleMovement();
        HandleCrouch();
    }

    //Inputs

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpPressed = true;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        isCrouching = context.ReadValueAsButton();
    }

    //Movement

    private void HandleMovement()
    {
        // Camera relative movement
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * moveInput.x + camForward * moveInput.y;
        move = Vector3.ClampMagnitude(move, 1f);

        // Speed according to state
        float targetSpeed = walkSpeed;
        if (isCrouching) targetSpeed = crouchSpeed;
        else if (isSprinting) targetSpeed = sprintSpeed;

        // Jump and Gravity
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        if (jumpPressed && controller.isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        jumpPressed = false; // Always resets

        velocity.y += gravity * Time.deltaTime;

        // Horizontal and Vertical Movement
        Vector3 finalMove = move * targetSpeed + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        float targetHeight = isCrouching ? crouchingHeight : standingHeight;
        float targetScaleY = isCrouching ? (crouchingHeight / standingHeight) : 1f;

        // Character Controller height changes
        controller.height = Mathf.Lerp(controller.height, targetHeight, heightChangeSpeed * Time.deltaTime);

        // Adjust center of model
        Vector3 center = controller.center;
        center.y = controller.height * 0.5f;
        controller.center = center;

        // Compress Mesh Visual and model "glued" to the floor
        if (visualTransform != null)
        {
            // Scale
            Vector3 scale = visualTransform.localScale;
            scale.y = Mathf.Lerp(scale.y, targetScaleY, heightChangeSpeed * Time.deltaTime);
            visualTransform.localScale = scale;

            // Position: Compensates for feet not to float above floor
                   
            Vector3 pos = visualTransform.localPosition;
            pos.y = Mathf.Lerp(pos.y, scale.y, heightChangeSpeed * Time.deltaTime);
            visualTransform.localPosition = pos;
        }
    }
    public void OnInteract(InputAction.CallbackContext context) //Mandatory for interactions
    {
        if (context.performed)
        {
            TryInteract();
        }
    }
    private void TryInteract()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward); // Raycast with camera
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            //Interacting
            Debug.Log("Interactuando con: " + hit.collider.name);

            // Interacts with GameObject that has Interactable layer
            var interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }
        public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        currentYaw += lookInput.x * lookSensitivity;
        currentPitch -= lookInput.y * lookSensitivity;
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);
        if (lookTarget != null)
        {
            lookTarget.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }
    }
}