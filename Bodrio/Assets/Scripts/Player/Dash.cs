using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // Necesario para la Corutina (ya lo tenías)

[RequireComponent(typeof(Rigidbody))]
public class Dash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    // --- SECCIÓN DE SONIDO AÑADIDA ---
    [Header("Sonido")]
    public AudioClip soundDash;
    [Range(0, 1)]
    public float volumeDash = 1.0f;

    private AudioSource audioSource;
    // --- FIN DE LA SECCIÓN AÑADIDA ---

    private Rigidbody rb;
    private PlayerInput playerInput;
    private bool isDashing = false;
    private bool canDash = true;

    private Vector3 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // --- AÑADIDO PARA INICIALIZAR EL AUDIO ---
        // Buscamos un AudioSource, si no existe, lo creamos.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // Importante: no queremos que suene al empezar
        }
        // --- FIN DE LO AÑADIDO ---
    }

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }
    void OnEnable()
    {
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
        playerInput.actions["Dash"].performed += OnDash;
    }

    void OnDisable()
    {
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;
        playerInput.actions["Dash"].performed -= OnDash;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (isDashing)
            return;

        Vector2 input = ctx.ReadValue<Vector2>();

        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDirection = (camForward * input.y + camRight * input.x).normalized;
        }
        else
        {
            moveDirection = new Vector3(input.x, 0, input.y).normalized;
        }
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (canDash && !isDashing && moveDirection != Vector3.zero)
            StartCoroutine(DoDash());
    }

    private System.Collections.IEnumerator DoDash()
    {
        canDash = false;
        isDashing = true;

        // --- AÑADIDO: REPRODUCIR SONIDO DE DASH ---
        if (soundDash != null)
        {
            audioSource.PlayOneShot(soundDash, volumeDash);
        }
        // --- FIN DE LO AÑADIDO ---

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(moveDirection * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}