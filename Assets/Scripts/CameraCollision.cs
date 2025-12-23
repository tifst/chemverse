using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("References")]
    public Transform target;           // player
    public Vector3 offset = new Vector3(0, 2.0f, -4.0f);

    [Header("Settings")]
    public float smoothSpeed = 10f;
    public float collisionRadius = 0.3f;
    public LayerMask collisionMask;    // layer dinding, terrain, dll

    private Vector3 desiredCameraPos;

    void LateUpdate()
    {
        if (!target) return;

        // posisi kamera ideal (tanpa halangan)
        desiredCameraPos = target.position + target.TransformDirection(offset);

        // arah dari player ke kamera
        Vector3 dir = (desiredCameraPos - target.position).normalized;
        float desiredDistance = offset.magnitude;

        // raycast untuk cek dinding di antara player dan kamera
        if (Physics.SphereCast(target.position, collisionRadius, dir, out RaycastHit hit, desiredDistance, collisionMask))
        {
            // kamera berhenti sebelum nabrak dinding
            float distance = hit.distance - 0.1f;
            desiredCameraPos = target.position + dir * Mathf.Max(distance, 0.5f);
        }

        // posisi halus
        transform.position = Vector3.Lerp(transform.position, desiredCameraPos, Time.deltaTime * smoothSpeed);
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(target.position, collisionRadius);
        Gizmos.DrawLine(target.position, desiredCameraPos);
    }
}