using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class LevelGoal : MonoBehaviour
{
    [Header("Meta")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float loadDelay = 1f;          // Segundos antes de cambiar de escena

    [Header("Siguiente nivel")]
    [SerializeField] private bool loadNextSceneInBuild = true; // Carga la siguiente escena según el Build Settings
    [SerializeField] private string specificSceneName = "";    // Si lo llenas, carga esta escena en vez de la siguiente

    private bool levelCompleted;

    private void Reset()
    {
        // Al añadir el script, el collider se vuelve Trigger automáticamente
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelCompleted || !other.CompareTag(playerTag)) return;
        levelCompleted = true;

        // Quitar el control al jugador y frenarlo
        PlayerMovement2D movement = other.GetComponent<PlayerMovement2D>();
        if (movement != null) movement.enabled = false;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Debug.Log("¡Nivel completado!");
        Invoke(nameof(LoadNext), loadDelay);
    }

    private void LoadNext()
    {
        if (!string.IsNullOrEmpty(specificSceneName))
        {
            SceneManager.LoadScene(specificSceneName);
            return;
        }

        if (loadNextSceneInBuild)
        {
            int next = SceneManager.GetActiveScene().buildIndex + 1;

            if (next < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(next);
            else
                Debug.Log("No hay más niveles en el Build Settings. ¡Juego completado!");
        }
    }
}