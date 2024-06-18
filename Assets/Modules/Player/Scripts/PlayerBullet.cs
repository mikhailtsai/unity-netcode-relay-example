using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerBullet : NetworkBehaviour
{
    public float speed;
    public Vector2 direction;
    public Collider2D ownerCollider;

    private new Collider2D collider;
    private new Rigidbody2D rigidbody;

    void Awake()
    {
        collider = GetComponent<Collider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (IsServer)
        {
            rigidbody.velocity = direction * speed;

            Physics2D.IgnoreCollision(collider, ownerCollider);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
        {
            if (collision.collider.CompareTag("Player"))
            {
                rigidbody.velocity = Vector2.zero;
                rigidbody.isKinematic = true;

                StopAllCoroutines();
                StartCoroutine(ExplodeAfterDelay(0.3f));
            }
            else
            {
                StartCoroutine(ExplodeAfterDelay(1f));
            }
        }
    }

    void Explode()
    {
        Destroy(gameObject);
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }
}
