using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 20f;       // Fuerza del empujón
    public float dashDuration = 0.2f;   // Cuánto dura el impulso
    public float dashCooldown = 1f;     // Tiempo entre dashes

    [Header("Camera Reference")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private bool isDashing = false;
    private bool canDash = true;

    private Vector3 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }
    void OnEnable()
    {
        // Conecta eventos del Input System
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
        // Si estás dashing, ignora input de movimiento
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

        // Aplica una fuerza instantánea hacia la dirección actual
        rb.linearVelocity = Vector3.zero; // Evita sumar velocidad previa
        rb.AddForce(moveDirection * dashForce, ForceMode.VelocityChange);

        // Espera a que termine la duración del dash
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        // Espera el cooldown antes de permitir otro dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}