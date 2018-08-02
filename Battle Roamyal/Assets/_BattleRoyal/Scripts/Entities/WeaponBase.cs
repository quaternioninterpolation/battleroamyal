// // --
// // Author: Josh van den Heever
// // Date: 31/07/2018 @ 6:21 p.m.
// // --
using UnityEngine;
using System.Collections;

public abstract class WeaponBase : Bolt.EntityEventListener<IBRWeapon>
{
    public int damage = 10;
    public float fireRate = 5f;
    public Transform owner;

    protected float shootTimer;

    public override void Attached()
    {
        base.Attached();

        //state
    }

    public abstract void Fire();

    public virtual bool CanFire()
    {
        return shootTimer <= 0f;
    }

    protected virtual void Update()
    {
        shootTimer = Mathf.Max(0f, shootTimer - Time.deltaTime);
    }

    protected virtual void ResetShootTimer()
    {
        shootTimer = 1.0f / fireRate;
    }

    public void SetFacingDirection(bool facingRight)
    {
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * (facingRight ? 1f : -1f);
    }
}
