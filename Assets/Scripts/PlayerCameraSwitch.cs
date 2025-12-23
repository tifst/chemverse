using UnityEngine;

public class PlayerCameraSwitchBackup : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    [Header("Camera POV Anchors")]
    public Transform firstPersonAnchor;
    public Transform thirdPersonAnchor;
    public Transform tpvPivotX;

    public float switchSpeed = 8f;
    public KeyCode switchKey = KeyCode.V;

    private float fpvXRotation = 0f;
    private float tpvXRotation = 0f;

    private bool isFirstPerson = true;
    private Transform targetAnchor;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        targetAnchor = firstPersonAnchor;

        // initial snap FPV
        transform.position = firstPersonAnchor.position;
        transform.rotation = firstPersonAnchor.rotation;
    }

    void LateUpdate()
    {
        HandleMouseLook();
        HandlePOVSwitch();
        FollowAnchor();
    }

    // ================================ MOUSE LOOK ================================
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Y-rotation: ALWAYS player
        playerBody.Rotate(Vector3.up * mouseX);

        if (isFirstPerson)
        {
            // ✅ FPV ROTATION X — HARUS BEKERJA 100%
            float targetX = fpvXRotation - mouseY;
            targetX = Mathf.Clamp(targetX, -60f, 60f);
            fpvXRotation = Mathf.Lerp(fpvXRotation, targetX, Time.deltaTime * 20f);
            firstPersonAnchor.localRotation = Quaternion.Euler(fpvXRotation, 0f, 0f);

            // HANYA kamera yang diputar → dijamin berfungsi
            transform.localRotation = Quaternion.Euler(fpvXRotation, 0f, 0f);
        }
        else
        {
            // ✅ TPS limited rotation via pivot
            tpvXRotation -= mouseY;
            tpvXRotation = Mathf.Clamp(tpvXRotation, 10f, 40f);
            tpvPivotX.localRotation = Quaternion.Euler(tpvXRotation, 0f, 0f);
        }
    }

    // ================================ SWITCH POV ================================
    void HandlePOVSwitch()
    {
        if (Input.GetKeyDown(switchKey))
        {
            isFirstPerson = !isFirstPerson;
            targetAnchor = isFirstPerson ? firstPersonAnchor : thirdPersonAnchor;

            // JANGAN RESET fpvXRotation = 0 (ini bikin FPV nge-bug)
        }
    }

    // ================================ FOLLOW ANCHOR ================================
    void FollowAnchor()
    {
        if (isFirstPerson)
        {
            // ✅ FPV SNAP (tidak smooth)
            transform.position = firstPersonAnchor.position;
            return;
        }

        // ✅ TPS follow (smooth)
        transform.position = Vector3.Lerp(
            transform.position,
            thirdPersonAnchor.position,
            Time.deltaTime * switchSpeed
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            thirdPersonAnchor.rotation,
            Time.deltaTime * switchSpeed
        );
    }
}
