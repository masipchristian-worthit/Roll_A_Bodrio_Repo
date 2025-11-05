using UnityEngine;

public class door_open : MonoBehaviour
{
    public Animator animator;
    public string nombretrigger = "open";
    public int requiredCoins = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCoins playerCoins = other.GetComponent<PlayerCoins>();

            if (playerCoins != null)
            {
                if (playerCoins.totalCoins >= requiredCoins)
                {
                    animator.SetTrigger(nombretrigger);
                }
                else
                {
                }
            }
        }
    }
}