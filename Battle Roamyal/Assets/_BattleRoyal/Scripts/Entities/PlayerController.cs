// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 6:51 p.m.
// // --
using UnityEngine;
using System.Collections;
using Bolt;

public class PlayerController : Bolt.EntityBehaviour<IBRPlayerState>
{
    public Animator animator;
    public Rigidbody2D rigidbody2D;
    public SpriteRenderer spriteRenderer;
    public Transform weaponRightSlot;
    public Transform weaponLeftSlot;

    public WeaponBase currentWeapon;

    public float acceleration;
    public float topSpeed;
    public float jumpForce;
    public float bottomOffsetY;
    public float checkRadius;

    public string speedAnimParam = "speed";
    public string isFallingAnimParam = "isFalling";

    public bool isGrounded = false;

    public override void Attached()
    {
        base.Attached();

        state.SetTransforms(state.transform, transform);
        state.AddCallback("weapon", OnWeaponUpdated);
        state.AddCallback("isFacingRight", OnWeaponUpdated);
    }

    private void OnWeaponUpdated()
    {
        if (state.weapon != null)
        {
            WeaponBase weapon = state.weapon.GetComponent<WeaponBase>();
            weapon.transform.SetParent(state.isFacingRight ? weaponRightSlot : weaponLeftSlot);
            weapon.transform.localPosition = Vector3.zero;
            weapon.SetFacingDirection(state.isFacingRight);
        }
    }

    public override void SimulateOwner()
    {
        base.SimulateOwner();

        //Update stuff
    }

    public override void SimulateController()
    {
        base.SimulateController();

        IPlayerMoveCommandInput moveCommand = PlayerMoveCommand.Create();

        Vector2 move = new Vector2();

        if (Input.GetKey(KeyCode.A) && isGrounded)
        {
            //Left move
            move += Vector2.left * acceleration;
        }

        if (Input.GetKey(KeyCode.D) && isGrounded)
        {
            //Right move
            move += Vector2.right * acceleration;
        }

        moveCommand.velocity = move;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            moveCommand.jump = true;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            moveCommand.fire = true;
        }

        entity.QueueInput(moveCommand);
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        base.ExecuteCommand(command, resetState);

        if (entity.isOwner)
        {
            IPlayerMoveCommandInput cmd = (IPlayerMoveCommandInput)command;

            state.isFacingRight = cmd.velocity.x >= 0;

            rigidbody2D.AddForce(cmd.velocity);
            if (rigidbody2D.velocity.magnitude > topSpeed)
            {
                rigidbody2D.velocity = rigidbody2D.velocity.normalized * topSpeed;
            }

            if (cmd.jump && isGrounded)
            {
                rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }

            if (cmd.fire)
            {
                if (currentWeapon != null)
                {
                    currentWeapon.Fire();
                }
            }
        }
    }

    private void Update()
    {
        Vector2 velocity = rigidbody2D.velocity;

        animator.SetFloat(speedAnimParam, velocity.magnitude);
        bool isFacingLeft = velocity.x < 0;
        spriteRenderer.flipX = isFacingLeft;

        //Check if we are grounded
        Vector2 checkPos = transform.position + Vector3.down * bottomOffsetY;
        Collider2D[] touchingObjects = Physics2D.OverlapCircleAll(checkPos, checkRadius);
        isGrounded = false;

        foreach (Collider2D curCollider in touchingObjects)
        {
            if (curCollider.gameObject != gameObject)
            {
                isGrounded = true;
                break;
            }
        }

        animator.SetBool(isFallingAnimParam, !isGrounded);
    }

    public void SV_AttachWeapon(WeaponBase weapon)
    {
        if (!entity.IsOwner()) return;

        state.weapon = weapon.entity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.red : Color.blue;
        Gizmos.DrawSphere(transform.position + Vector3.down * bottomOffsetY, checkRadius);
    }
}
