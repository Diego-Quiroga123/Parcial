using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;              // Si se deja vacío, busca el objeto con tag "Player"
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);

    [Header("Suavizado")]
    [SerializeField] private float smoothTime = 0.15f;      // 0 = sigue de inmediato

    [Header("Seguir ejes")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;

    [Header("Límites del nivel (opcional)")]
    [SerializeField] private bool useLimits = false;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 10f;

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
            else Debug.LogWarning("CameraFollow2D: no se encontró ningún objeto con el tag 'Player'.");
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + offset;

        // Mantener el eje que no se sigue
        if (!followX) desired.x = transform.position.x;
        if (!followY) desired.y = transform.position.y;

        // Limitar la posición de la cámara
        if (useLimits)
        {
            desired.x = Mathf.Clamp(desired.x, minX, maxX);
            desired.y = Mathf.Clamp(desired.y, minY, maxY);
        }

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (!useLimits) return;

        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}