using UnityEngine;

public class PICKUP_COINS : MonoBehaviour
{
    public float attractionSpeed = 5f;
    public float collectDistance = 0.4f;
    private Transform target;
    private bool isAttracted = false;
    public int coinValue = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.transform;
            isAttracted = true;
        }
    }

    private void FixedUpdate()
    {
        if (isAttracted && target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * attractionSpeed);

            if (Vector3.Distance(transform.position, target.position) < collectDistance)
            {
                Collect();
            }
        }
    }

    private void Collect()
    {
        PlayerCoins playerCoins = target.GetComponent<PlayerCoins>();
        if (playerCoins != null)
        {
            playerCoins.AddCoins(coinValue);
        }

        gameObject.SetActive(false);
    }
}