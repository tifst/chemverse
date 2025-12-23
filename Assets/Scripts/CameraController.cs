using UnityEngine;

/// <summary>
/// Camera Controller yang bisa toggle antara Third Person dan First Person view.
/// Pasang di GameObject kosong (parent dari Main Camera).
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target & Kamera")]
    public Transform player;                // drag Player ke sini lewat inspector
    public Transform firstPersonPivot;      // posisi kepala player (untuk FPS)
    public Camera mainCamera;

    [Header("Opsi Kamera")]
    public bool canZoom = true;
    public float sensitivity = 5f;
    public Vector2 cameraLimit = new Vector2(-45, 40);

    [Header("Third Person Offset")]
    public Vector3 thirdPersonOffset = new Vector3(0, 2.0f, -4.0f);
    private Vector3 offsetTPV = new Vector3(0, 2.0f, -3.0f);

    [Header("First Person Offset")]
    public Vector3 firstPersonOffset = new Vector3(0, 0.2f, 0.0f);
    private Vector3 offsetFPV = new Vector3(0, 0.0f, 0.0f);

    [Header("Transisi")]
    public float switchSpeed = 5f;

    public bool isFirstPerson = false;     // mode aktif
    private float mouseX;
    private float mouseY;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("❌ Player belum di-assign ke CameraController!");
            enabled = false;
            return;
        }

        if (mainCamera == null)
            mainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleViewToggle();
        HandleCameraRotation();
        HandleZoom();
        FollowPlayer();
    }

    void HandleViewToggle()
    {
        // Tekan V untuk toggle FPS/TPV
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
        }
    }

    void HandleCameraRotation()
    {
        float mouseInputX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseInputY = Input.GetAxis("Mouse Y") * sensitivity;

        mouseX += mouseInputX;
        mouseY -= mouseInputY;
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        // rotasi player mengikuti horizontal mouse
        player.rotation = Quaternion.Euler(0, mouseX, 0);

        // rotasi kamera (x = vertikal, y = horizontal)
        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }

    void HandleZoom()
    {
        if (!canZoom) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            mainCamera.fieldOfView -= scroll * sensitivity * 2;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, 30f, 90f);
        }
    }

    void FollowPlayer()
    {
        Vector3 targetPos;

        if (isFirstPerson)
        {
            // Kamera di kepala player
            // Turunkan Y jadi 0 (sejajar kepala)
            mainCamera.transform.localPosition = offsetFPV;

            Vector3 headPos = firstPersonPivot != null
                ? firstPersonPivot.position
                : player.position + Vector3.up * 1.6f;

            // Posisi kamera sedikit ke depan agar tidak menembus model
            targetPos = headPos + player.TransformDirection(firstPersonOffset);
        }
        else
        {
            mainCamera.transform.localPosition = offsetTPV;

            // Kamera di belakang player (third-person)
            // Y turun ke bawah (-4) dan sedikit ke belakang
            Vector3 behindPlayer = player.position + player.TransformDirection(thirdPersonOffset);
            targetPos = new Vector3(behindPlayer.x, behindPlayer.y - 4f, behindPlayer.z);
        }

        // Transisi halus antara mode
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * switchSpeed);
    }
}