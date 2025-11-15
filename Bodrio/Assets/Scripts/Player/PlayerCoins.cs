using UnityEngine;
using TMPro;

public class PlayerCoins : MonoBehaviour
{
    public int totalCoins = 0;
    public TextMeshProUGUI coinsText;

    // --- SECCIÓN DE SONIDO AÑADIDA ---
    [Header("Sonido")]
    public AudioClip soundCoinPickup;
    [Range(0, 1)]
    public float volumeCoinPickup = 1.0f;

    private AudioSource audioSource;
    // --- FIN DE LA SECCIÓN AÑADIDA ---


    // --- AÑADIDO: Awake para inicializar el AudioSource ---
    void Awake()
    {
        // Buscamos un AudioSource, si no existe, lo creamos.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // Importante
        }
    }
    // --- FIN DE LO AÑADIDO ---

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateUI();

        // --- AÑADIDO: REPRODUCIR SONIDO DE MONEDA ---
        // Suena solo si se añade una cantidad positiva
        if (soundCoinPickup != null && amount > 0)
        {
            audioSource.PlayOneShot(soundCoinPickup, volumeCoinPickup);
        }
        // --- FIN DE LO AÑADIDO ---
    }

    private void UpdateUI()
    {
        if (coinsText != null)
        {
            coinsText.text = "Coins: " + totalCoins;
        }
    }

    private void Start()
    {
        UpdateUI(); // Muestra 0 al inicio 
    }
}