// // --
// // Author: Josh van den Heever
// // Date: 31/07/2018 @ 6:28 p.m.
// // --
using UnityEngine;
using System.Collections;

public class ProjectileWeapon : WeaponBase
{
    public Transform muzzleSpawnPos;
    public Transform muzzleAim;

    public GameObject projectilePrefab;

    public override void Fire()
    {
        if (!entity.IsOwner()) return;

        if (CanFire())
        {
            GameObject projectile = BoltNetwork.Instantiate(projectilePrefab, muzzleSpawnPos.position, Quaternion.identity);
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript)
            {
                projectileScript.SetData(damage, GetDirection(), owner);
            }

            WeaponFired.Create(entity).Send();
        }
    }

    public override void OnEvent(WeaponFired evnt)
    {
        base.OnEvent(evnt);

        //TODO: Spawn a muzzle flash.
        ResetShootTimer();
    }

    protected Vector3 GetDirection()
    {
        Vector3 result = muzzleAim.position - muzzleSpawnPos.position;
        return result.normalized;
    }
}
