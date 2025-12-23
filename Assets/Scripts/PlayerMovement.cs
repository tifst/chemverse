using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float crouchSpeed = 1.8f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1f;
    public float gravity = -15f;

    [Header("Crouch Settings")]
    public float standHeight = 1.8f;
    public float crouchHeight = 1.2f;
    public float crouchSmooth = 10f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float cameraCrouchOffset = 0.4f;

    [Header("Mesh (Visual Body)")]
    public Transform meshTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;

    private float originalCamY;
    private Vector3 meshOriginalPos;

    private float standMeshY;
    private float crouchMeshY;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        controller.height = standHeight;
        controller.center = new Vector3(0f, standHeight / 2f, 0f);

        if (cameraTransform != null)
            originalCamY = cameraTransform.localPosition.y;

        if (meshTransform != null)
        {
            meshOriginalPos = meshTransform.localPosition;
            standMeshY = meshOriginalPos.y;
            crouchMeshY = standMeshY - (standHeight - crouchHeight);
        }
    }

    void Update()
    {
        HandleMovement();
        HandleCrouch();
    }

    void HandleMovement()
    {
        // Better grounded check
        isGrounded = Physics.Raycast(
            transform.position + Vector3.up * 0.2f,
            Vector3.down,
            0.3f
        );

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float speed = isCrouching ? crouchSpeed :
                      (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            isCrouching = true;
        else
        {
            // prevent standing if ceiling is too close
            bool blocked = Physics.Raycast(
                cameraTransform.position,
                Vector3.up,
                standHeight - crouchHeight + 0.1f
            );

            if (!blocked)
                isCrouching = false;
        }

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchSmooth);
        controller.center = new Vector3(0f, controller.height / 2f, 0f);

        if (cameraTransform != null)
        {
            float targetY = isCrouching ? (originalCamY - cameraCrouchOffset) : originalCamY;
            Vector3 camPos = cameraTransform.localPosition;
            camPos.y = Mathf.Lerp(camPos.y, targetY, Time.deltaTime * crouchSmooth);
            cameraTransform.localPosition = camPos;
        }

        if (meshTransform != null)
        {
            float targetMeshY = isCrouching ? crouchMeshY : standMeshY;

            Vector3 pos = meshTransform.localPosition;
            pos.y = Mathf.Lerp(pos.y, targetMeshY, Time.deltaTime * crouchSmooth);
            meshTransform.localPosition = pos;
        }
    }
}