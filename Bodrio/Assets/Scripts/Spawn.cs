using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnID;

    private void Start()
    {
        if (SceneTransitionManager.LastDoorID == spawnID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = transform.position;
                player.transform.rotation = transform.rotation;
            }
        }
    }
}