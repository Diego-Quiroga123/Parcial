using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [Header("Muerte")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float fallLimitY = -15f;     // Si el jugador baja de esta Y, muere
    [SerializeField] private float restartDelay = 1f;     // Segundos antes de reiniciar el nivel

    private bool isDead;
    private Rigidbody2D rb;
    private PlayerMovement2D movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement2D>();
    }

    private void Update()
    {
        // Caída al vacío
        if (!isDead && transform.position.y < fallLimitY)
            Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(enemyTag))
            Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
            Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Quitar el control al jugador
        if (movement != null) movement.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Invoke(nameof(RestartLevel), restartDelay);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}