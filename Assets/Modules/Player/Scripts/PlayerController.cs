using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Services.Authentication;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : NetworkBehaviour
{
    public float speed = 5f;
    public float jumpForce = 6.0f;
    public Color ownerColor = new Color(100f, 220f, 140f, 1f);
    public Color othersColor = new Color(100f, 140, 220f, 1f);
    public TMP_Text playerName;
    public GameObject bulletPrefab;

    private bool stopped = false;

    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (IsServer)
        {
            transform.position = new Vector2(Random.Range(-5, 5), 0);
            rigidbody.isKinematic = false;
        }

        if (IsOwner)
        {
            Camera.main.GetComponent<CameraFollow>().target = transform;

            spriteRenderer.color = ownerColor;
        }
        else
        {
            spriteRenderer.color = othersColor;
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            HandleShooting();

            if (IsGrounded())
            {
                HandleMovement();
            }
        }

        if (IsServer && IsGrounded() && stopped)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x * 0.5f, rigidbody.velocity.y);
        }
    }

    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            Vector3 objectPosition = transform.position;

            Vector2 direction = new Vector2(
                mouseWorldPosition.x - objectPosition.x,
                mouseWorldPosition.y - objectPosition.y
            );
            direction.Normalize();

            Debug.Log("shooting");
            Debug.Log(direction);

            HandleShootingServerRpc(direction);
        }
    }

    void HandleMovement()
    {
        float xInput = Input.GetAxis("Horizontal");
        bool isJumpPressed = Input.GetAxis("Vertical") > 0;
        bool isNoInput = Mathf.Approximately(xInput, 0) && !isJumpPressed;

        if (isNoInput && stopped) return;

        stopped = isNoInput; // Чтобы 1 раз мы послали на сервер нулевой input

        HandleMovementServerRpc(xInput, isJumpPressed);
    }

    [Rpc(SendTo.Server)]
    void HandleShootingServerRpc(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, (Vector2)transform.position + new Vector2(0, 1.75f), Quaternion.identity);
        NetworkObject networkObject = bullet.GetComponent<NetworkObject>();
        PlayerBullet playerBullet = bullet.GetComponent<PlayerBullet>();
        playerBullet.direction = direction;
        playerBullet.ownerCollider = collider;
        networkObject.Spawn();
        Debug.Log("Bullet spawned");
    }

    [Rpc(SendTo.Server)]
    void HandleMovementServerRpc(float xInput, bool isJumpPressed)
    {
        Vector2 move = rigidbody.velocity;

        if (IsGrounded())
        {
            move.x = xInput * speed;

            if (isJumpPressed)
            {
                move.y = jumpForce;
                move.x = (move.x > 0f ? 1f : move.x < 0f ? -1f : 0f) * speed * 0.75f;
            }
        }

        rigidbody.velocity = move;
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            new Vector2(transform.position.x, transform.position.y - 0.5f),
            new Vector2(1f, 0.01f),
            0f,
            Vector2.down,
            0,
            LayerMask.GetMask("Solid")
        );

        return hit.collider != collider && hit.collider != null;
    }
}
