using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement2D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private bool startMovingLeft = true;   // Empieza hacia -X

    [Header("Activación")]
    [SerializeField] private float activationDistance = 8f; // Se activa cuando el jugador está a esta distancia
    [SerializeField] private bool stayActiveOnceTriggered = true; // Si es false, se detiene cuando el jugador se aleja

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer; // Si se deja vacío, se busca solo
    [SerializeField] private bool spriteFacesLeft = true;   // Los enemigos de Pixel Adventure suelen mirar a la izquierda

    [Header("Opcional")]
    [SerializeField] private bool destroyWhenFarLeft = true;
    [SerializeField] private float destroyAtX = -30f;

    private Rigidbody2D rb;
    private Transform player;
    private int direction;      // -1 = izquierda, 1 = derecha
    private bool isActive;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        direction = startMovingLeft ? -1 : 1;
        UpdateSpriteFlip();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        else Debug.LogWarning("EnemyMovement2D: no se encontró ningún objeto con el tag 'Player'.");
    }

    private void Update()
    {
        CheckActivation();

        if (destroyWhenFarLeft && transform.position.x < destroyAtX)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        float xVelocity = isActive ? direction * speed : 0f;
        rb.linearVelocity = new Vector2(xVelocity, rb.linearVelocity.y);
    }

    private void CheckActivation()
    {
        if (player == null) return;

        bool playerIsNear = Vector2.Distance(transform.position, player.position) <= activationDistance;

        if (playerIsNear) isActive = true;
        else if (!stayActiveOnceTriggered) isActive = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckWallHit(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckWallHit(collision);
    }

    private void CheckWallHit(Collision2D collision)
    {
        // Ignorar al jugador
        if (collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // La normal apunta desde el objeto golpeado hacia este enemigo.
            // Solo giramos si el choque es lateral Y el enemigo se está moviendo
            // hacia ese objeto (así nunca gira dos veces por el mismo golpe).
            if (Mathf.Abs(contact.normal.x) > 0.5f && contact.normal.x * direction < -0.5f)
            {
                TurnAround();
                return;
            }
        }
    }

    private void TurnAround()
    {
        direction *= -1;
        UpdateSpriteFlip();

        // Aplicar la nueva velocidad de inmediato para que se despegue de la pared
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    private void UpdateSpriteFlip()
    {
        if (spriteRenderer == null) return;

        bool movingLeft = direction < 0;
        spriteRenderer.flipX = spriteFacesLeft ? !movingLeft : movingLeft;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}