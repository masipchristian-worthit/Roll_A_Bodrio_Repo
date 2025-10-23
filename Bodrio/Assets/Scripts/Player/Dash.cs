using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashForce = 20f;       // Fuerza del empuj�n
    public float dashDuration = 0.2f;   // Cu�nto dura el impulso
    public float dashCooldown = 1f;     // Tiempo entre dashes

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
        Vector2 input = ctx.ReadValue<Vector2>();
        moveDirection = new Vector3(input.x, 0, input.y).normalized;
    }

    private void OnDash(InputAction.CallbackContext ctx)
    {
        if (canDash && moveDirection != Vector3.zero)
            StartCoroutine(DoDash());
    }

    private System.Collections.IEnumerator DoDash()
    {
        canDash = false;
        isDashing = true;

        // Aplica una fuerza instant�nea hacia la direcci�n actual
        rb.linearVelocity = Vector3.zero; // evita sumar velocidad previa
        rb.AddForce(moveDirection * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        // Deja que el jugador desacelere naturalmente
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
