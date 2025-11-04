using UnityEngine;

public class PlayAnimationOnTrigger : MonoBehaviour
{
    public Animator animator;          // El Animator del objeto
    public string animationTriggerName; // Nombre del trigger o animación

    private void OnTriggerEnter(Collider other)
    {
        // Comprueba si el objeto que entra es el jugador
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador entró al área, reproduciendo animación...");
            animator.SetTrigger(animationTriggerName);
        }
    }
}