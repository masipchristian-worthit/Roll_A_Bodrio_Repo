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
    public Vector3 direction; //Direcci�n de movimiento para  el movimiento f�sico

    [Header("Jump Parameters")]
    public float jumpForce; //Potencia de salto de personaje
    public bool isGrounded = true; //Determina si el pj est� en el suelo (Limita el salto)

    [Header("Double Jump Parameters")]
    public bool canDoubleJump = false;  //activado por el pickup
    public bool hasJumpedOnce = false; //controla si ya hizo el primer salto
    private bool jumpPressed = false; //Para evitar doble salto autom�tico
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Ladder Settings")]
    public bool isClimbing = false;
    public float climbSpeed = 4f;
    private float originalGravity;

    void Start()
    {

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        originalGravity = Physics.gravity.y;
    }

    // Update is called once per frame
    void Update()
    {
        //CinematicMovement();

    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            ClimbLadder();
        }
      

            //Tiempo constante del motor de f�sicas
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

    void ClimbLadder()
    {
        if (moveInput.y <= 0.1f)
        {
            playerRb.useGravity = true;
            return;
        }
        playerRb.useGravity = false;

        Vector3 climbVelocity = new Vector3(0, climbSpeed, 0);
        playerRb.linearVelocity = climbVelocity; 

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
            jumpPressed = true; // bot�n presionado
        }

        if (context.canceled)
        {
            jumpPressed = false; // bot�n soltado
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
            // Segundo salto manual (solo si vuelve a presionar el bot�n)
            else if (canDoubleJump && hasJumpedOnce && jumpPressed)
            {
                hasJumpedOnce = false;
                Jump();
            }

            // Previene que salte dos veces en la misma pulsaci�n
            jumpPressed = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = true;
            playerRb.useGravity = false;
            playerRb.angularVelocity = Vector3.zero;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false;
            playerRb.useGravity = true;
            playerRb.constraints = RigidbodyConstraints.None;
        }
    }
}