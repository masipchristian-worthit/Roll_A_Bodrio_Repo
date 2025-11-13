using UnityEngine;
using TMPro;

public class PlayerCoins : MonoBehaviour
{
    public int totalCoins = 0;
    public TextMeshProUGUI coinsText;

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        UpdateUI();
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