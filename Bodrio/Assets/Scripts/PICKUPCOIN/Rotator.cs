using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float velocidad = 100f;

    void Update()
    {
        // Rotar solo este objeto en X
        transform.Rotate(velocidad * Time.deltaTime, 0f, 0f);
    }
}