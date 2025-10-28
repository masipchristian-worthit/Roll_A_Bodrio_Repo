using UnityEngine;

public class PICKUP_COINS : MonoBehaviour
{
    public float attractionSpeed;
    public float collectDistance = 0.4f;
    public Transform target;
    private bool isAttracted = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            target = other.transform;
            isAttracted = true;
            //GetComponent<Collider>().enabled = false;
        }
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isAttracted && target != null)
        {
            transform.position = Vector3.Lerp(target.position, transform.position, Time.deltaTime * attractionSpeed);
        }
        if (Vector3.Distance(transform.position, target.position) <collectDistance)
        {
          Collect();

        }
    }

    private void Collect()
    {
        gameObject.gameObject.SetActive(false);
    }
}
