using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DVDLogoBounce : MonoBehaviour, IPointerClickHandler
{
    public RectTransform movingImage;
    public float speed = 200f;

    public AudioSource audioSource;
    public AudioClip clickSound;

    private Vector2 direction;
    private RectTransform canvasRect;

    void Start()
    {
        canvasRect = movingImage.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        direction = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        movingImage.anchoredPosition += direction * speed * Time.deltaTime;

        Vector2 pos = movingImage.anchoredPosition;
        Vector2 halfSize = movingImage.sizeDelta / 2f;
        Vector2 canvasHalf = canvasRect.sizeDelta / 2f;

        if (pos.x + halfSize.x > canvasHalf.x || pos.x - halfSize.x < -canvasHalf.x)
            direction.x *= -1;

        if (pos.y + halfSize.y > canvasHalf.y || pos.y - halfSize.y < -canvasHalf.y)
            direction.y *= -1;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}