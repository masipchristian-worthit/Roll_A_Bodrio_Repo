using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Editor References")]
    public Rigidbody playerRb; //Ref al rigidbody del player (Permite modificar gravedad, movimientos fisicos...)
    public Transform cameraTransform;

    [Header("Movement Parameters")]
    public float speed = 10; //Velocidad del personaje
    public Vector2 moveInput; //Almacen del input de movimiento en la vida real
    public Vector3 direction; //Dirección de movimiento para  el movimiento físico

    [Header("Jump Parameters")]
    public float jumpForce; //Potencia de salto de personaje
    public bool isGrounded = true; //Determina si el pj está en el suelo (Limita el salto)

    [Header("Double Jump Parameters")]
    public bool canDoubleJump = false;  //activado por el pickup
    public bool hasJumpedOnce = false; //controla si ya hizo el primer salto
    private bool jumpPressed = false; //Para evitar doble salto automático
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //CinematicMovement();

    }

    private void FixedUpdate()
    {
        //Tiempo constante del motor de físicas
        PhisycalMovement();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            hasJumpedOnce = false; //reset del salto doble
        }
        
    }

    void CinematicMovement()
    {
        transform.Translate(Vector3.right * speed * moveInput.x * Time.deltaTime);
        transform.Translate(Vector3.forward * speed * moveInput.y * Time.deltaTime);
    }

    void PhisycalMovement()
    {
        if (cameraTransform == null) return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        playerRb.AddForce(moveDir * speed);

        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }

    void Jump()
    {
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    //Eventos de Input
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpPressed = true; // botón presionado
        }

        if (context.canceled)
        {
            jumpPressed = false; // botón soltado
        }

        if (context.performed)
        {
            // Primer salto
            if (isGrounded)
            {
                isGrounded = false;
                hasJumpedOnce = true;
                Jump();
            }
            // Segundo salto manual (solo si vuelve a presionar el botón)
            else if (canDoubleJump && hasJumpedOnce && jumpPressed)
            {
                hasJumpedOnce = false;
                Jump();
            }

            // Previene que salte dos veces en la misma pulsación
            jumpPressed = false;
        }
    }
}