using UnityEngine;

public class PlayerCoins : MonoBehaviour
{
    public int totalCoins = 0;

    // Método para sumar monedas
    public void AddCoins(int amount)
    {
        totalCoins += amount;
        Debug.Log("Monedas totales: " + totalCoins); // Solo para ver en la consola
    }
}