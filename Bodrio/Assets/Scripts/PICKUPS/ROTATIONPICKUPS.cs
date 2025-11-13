using UnityEngine;

public class ROTATIONPICKUPS : MonoBehaviour
{
    public float rotationSpeed = 100f;

    void Update()
    {
        transform.Rotate( 0f, rotationSpeed * Time.deltaTime, 0f);
    }
}