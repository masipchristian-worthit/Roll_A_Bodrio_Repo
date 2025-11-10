using UnityEngine;
using TMPro;

public class CoinMeter : MonoBehaviour
{
    [Header("Referencia al PlayerCoins")]
    public PlayerCoins playerCoins;

    [Header("Texto donde se muestran las monedas")]
    public TextMeshProUGUI coinText;

    private int lastCoins;

    void Start()
    {
        UpdateUI();
        lastCoins = playerCoins.totalCoins;
    }

    void Update()
    {
        // Si cambió la cantidad de monedas, actualiza el texto
        if (playerCoins.totalCoins != lastCoins)
        {
            UpdateUI();
            lastCoins = playerCoins.totalCoins;
        }
    }

    void UpdateUI()
    {
        coinText.text = "Coins: " + playerCoins.totalCoins.ToString();
    }
}