using UnityEngine;

public class door_open : MonoBehaviour
{
    public Animator animator;
    public string nombretrigger = "open";

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger(nombretrigger);
        }
    }
}
