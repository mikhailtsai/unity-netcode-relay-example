using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PlayerBullet : NetworkBehaviour
{
    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private NetworkObject networkObject;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        networkObject = GetComponent<NetworkObject>();
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

    async void Explode()
    {
        rigidbody.isKinematic = true;

        ExplodeRpc();

        await Task.Delay(TimeSpan.FromSeconds(3f));

        networkObject.Despawn();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ExplodeRpc()
    {
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector2.zero;
        spriteRenderer.color = new Color(1f, 0f, 0f, 0.3f);
        transform.localScale = new Vector2(0.3f, 0.3f);
    }

    IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Explode();
    }
}
