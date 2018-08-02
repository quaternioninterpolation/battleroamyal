// // --
// // Author: Josh van den Heever
// // Date: 31/07/2018 @ 6:35 p.m.
// // --
using UnityEngine;
using System.Collections;
using MovementEffects;

public class Projectile : Bolt.EntityEventListener<IBRProjectile>
{
    public Transform spawnOnDeath;
    public float speed;

    protected int damage;
    protected CoroutineHandle destroyHandle;

    private void Awake()
    {
    }

    public override void Attached()
    {
        base.Attached();

        state.SetTransforms(state.transform, transform);

        if (entity.IsOwner())
        {
            destroyHandle = Timing.CallDelayed(1f, ()=>BoltNetwork.Destroy(gameObject));
        }
    }

    private void OnDestroy()
    {
        if (destroyHandle.IsValid)
        {
            Timing.KillCoroutines(destroyHandle);
        }
    }

    public void SetData(int damage, Vector2 direction, Transform owner)
    {
        this.damage = damage;

        //Angle to face
        float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation + 90f);

        //Set initial velocity
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        if (rigidbody2D != null)
        {
            rigidbody2D.velocity = direction.normalized * speed;
        }

        //Ignore collision between this and the owner
        Collider2D ownerCollider = owner.GetComponent<Collider2D>();
        if (ownerCollider != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), ownerCollider);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!entity.IsOwner()) return;

        var deathEvent = DeathEvent.Create(entity);
        deathEvent.Send();

        BoltNetwork.Destroy(gameObject);

        Hitbox otherHitbox = collision.collider.GetComponent<Hitbox>();
        if (otherHitbox)
        {
            otherHitbox.Damage(damage);
        }
    }

    public override void OnEvent(DeathEvent evnt)
    {
        base.OnEvent(evnt);
        Instantiate(spawnOnDeath, transform.position, Quaternion.identity);
    }
}
