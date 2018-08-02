// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 7:59 p.m.
// // --
using UnityEngine;
using System.Collections;

public class WeaponPickup : Bolt.EntityEventListener<IBRPickup>
{
    public Bolt.PrefabId weaponPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!entity.IsOwner()) return;

        //See if it was a player!
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            var weapon = SV_SpawnWeapon();
            player.SV_AttachWeapon(weapon);
        }
    }

    private WeaponBase SV_SpawnWeapon()
    {
        if (!entity.IsOwner()) return null;

        BoltEntity weaponEnt = BoltNetwork.Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        return weaponEnt.GetComponent<WeaponBase>();
    }
}
