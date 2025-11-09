using UnityEngine;

public class PickupDestroyWallTeeth : MonoBehaviour
{
    public string parentName = "AnimationWallsParent";
    public bool useTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (useTrigger && other.CompareTag("Player"))
            ActivatePickup();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!useTrigger && collision.gameObject.CompareTag("Player"))
            ActivatePickup();
    }

    private void ActivatePickup()
    {
        PlayerPickupState.TeethPickup = true;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("WallTeeth"))
            Destroy(obj);

        GameObject parent = GameObject.Find(parentName);
        if (parent != null)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.CompareTag("AnimationWallTeeth"))
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
        else
        {
        }

        Destroy(gameObject);
    }
}