using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    [Header("Muerte")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float fallLimitY = -15f;     // Si el jugador baja de esta Y, muere
    [SerializeField] private float restartDelay = 1f;     // Segundos antes de reiniciar el nivel

    [Header("Aplastar enemigos")]
    [SerializeField] private float stompBounceForce = 10f; // Rebote al aplastar un enemigo

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
        if (isDead || !collision.gameObject.CompareTag(enemyTag)) return;

        // Si el golpe viene desde arriba (la normal apunta hacia arriba), aplastamos al enemigo
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                StompEnemy(collision.gameObject);
                return;
            }
        }

        // Si fue un golpe lateral o desde abajo, el jugador muere
        Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Para enemigos o trampas configurados como Trigger: siempre matan
        if (!isDead && other.CompareTag(enemyTag))
            Die();
    }

    private void StompEnemy(GameObject enemyObject)
    {
        EnemyMovement2D enemy = enemyObject.GetComponentInParent<EnemyMovement2D>();
        if (enemy != null) enemy.Die();

        // Rebote del jugador
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceForce);
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