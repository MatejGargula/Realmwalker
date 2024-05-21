using UnityEngine;


// Movement for the whole party
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;

    [SerializeField] private GameObject
        player; // TODO: this should hold the whole party. OR the party members should have a "following" script.

    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private Transform cameraFocus;
    [SerializeField] private float cameraFocusDistance = 4.0f;
    private Vector2 movement;

    private void Start()
    {
        sr = player.GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        if (movement.x > 0.1f)
        {
            sr.flipX = false;
            cameraFocus.position = new Vector3(cameraFocusDistance, 2.5f, 0.0f) + player.transform.position;
        }
        else if (movement.x < -0.1f)
        {
            sr.flipX = true;
            cameraFocus.position = new Vector3(-cameraFocusDistance, 2.5f, 0.0f) + player.transform.position;
        }

        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
    }
}