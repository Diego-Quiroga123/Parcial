using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Fruit : MonoBehaviour
{
    [Header("Fruta")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private int value = 1;                 // Cuánto suma al contador

    [Header("Animación de recogida (opcional)")]
    [SerializeField] private bool useCollectedAnimation = false; // Requiere un Trigger llamado "Collected" en el Animator
    [SerializeField] private float destroyDelay = 0.4f;          // Cuánto dura la animación antes de destruirse

    private bool collected;

    private void Reset()
    {
        // Al añadir el script, el collider se vuelve Trigger automáticamente
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag(playerTag)) return;
        collected = true;

        if (FruitCounter.Instance != null)
            FruitCounter.Instance.AddFruit(value);

        Animator animator = GetComponent<Animator>();

        if (useCollectedAnimation && animator != null)
        {
            GetComponent<Collider2D>().enabled = false;
            animator.SetTrigger("Collected");
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}