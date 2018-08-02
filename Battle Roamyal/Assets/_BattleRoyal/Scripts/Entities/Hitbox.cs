// // --
// // Author: Josh van den Heever
// // Date: 31/07/2018 @ 6:37 p.m.
// // --
using UnityEngine;
using System.Collections;

public class Hitbox : Bolt.EntityEventListener<IEntityBase>
{
    public Transform spawnOnDeath;

    public void Damage(int amount)
    {
        if (!entity.isOwner) return;

        state.health = Mathf.Max(0, state.health - amount);
        
        if (state.health <= 0)
        {
            SV_Die();
        }
    }

    public void SV_Die()
    {
        if (!entity.isOwner) return;

        var deathEvent = DeathEvent.Create(entity);
        deathEvent.Send();

        BoltNetwork.Destroy(gameObject);
    }

    public override void OnEvent(DeathEvent evnt)
    {
        base.OnEvent(evnt);
        Instantiate(spawnOnDeath, transform.position, Quaternion.identity);
    }
}
