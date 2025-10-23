using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Editor References")]
    public Rigidbody playerRb; //Ref al rigidbody del player (Permite modificar gravedad, movimientos fisicos...)

    [Header("Movement Parameters")]
    public float speed = 10; //Velocidad del personaje
    public Vector2 moveInput; //Almacen del input de movimiento en la vida real
    public Vector3 direction; //Dirección de movimiento para  el movimiento físico

    [Header("Jump Parameters")]
    public float jumpForce; //Potencia de salto de personaje
    public bool isGrounded = true; //Determina si el pj está en el suelo (Limita el salto)

    [Header("Respawn configuration")]
    public float respawnLimit = -10f; //Limite inferior del que el pj respawnea
    public Transform respawnPoint; //Ref a la posición de respawneo
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //CinematicMovement();

        //Chequeo del respawn por altura
        if (transform.position.y <= respawnLimit)
        {
            Respawn();
        }

    }

    private void FixedUpdate()
    {
        //Tiempo constante del motor de físicas
        PhisycalMovement();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        
    }

    void CinematicMovement()
    {
        transform.Translate(Vector3.right * speed * moveInput.x * Time.deltaTime);
        transform.Translate(Vector3.forward * speed * moveInput.y * Time.deltaTime);
    }

    void PhisycalMovement()
    {
        playerRb.AddForce(Vector3.right * speed * moveInput.x);
        playerRb.AddForce(Vector3.forward * speed * moveInput.y);
    }

    void Jump()
    {
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void Respawn()
    {
        transform.position = respawnPoint.position; //Cambia la posicion del pj a la posición del punto respawn
        playerRb.linearVelocity = new Vector3(0, 0, 0); //Resetear la velocidad del rigidbody
    }

    //Eventos de Input
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded == true)
        {
            isGrounded = false;
            Jump();
        }
    }
}