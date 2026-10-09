using TMPro;
using UnityEngine;

public class FruitCounter : MonoBehaviour
{
    public static FruitCounter Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text counterText;          // El texto del Canvas donde se muestra el contador
    [SerializeField] private string prefix = "Frutas: ";
    [SerializeField] private bool showTotal = true;         // Muestra "3 / 10" en vez de solo "3"

    private int count;
    private int total;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Cuenta cuántas frutas hay en el nivel al empezar
        total = FindObjectsByType<Fruit>(FindObjectsSortMode.None).Length;
        UpdateUI();
    }

    public void AddFruit(int amount = 1)
    {
        count += amount;
        UpdateUI();
    }

    public int Count => count;

    private void UpdateUI()
    {
        if (counterText == null) return;

        counterText.text = showTotal
            ? prefix + count + " / " + total
            : prefix + count;
    }
}