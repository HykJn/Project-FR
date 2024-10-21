using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil_Small : Enemy
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerAttack"))
        {
            Hit(1);
        }
    }

    protected override void Idle()
    {
        curState = State.Idle;
        animator.Play("Idle");

        //To Chase
        if (_fDistanceToPlayer <= _iDistanceToChase)
        {
            curState = State.Chase;
        }
    }

    protected override void Chase()
    {
        curState = State.Chase;
        animator.Play("Chase");

        Pathfinding();

        if(pathList.Count > 1)
        {
            Vector2 movePos = new Vector2(pathList[1].x, pathList[1].y);
            Vector2 dir = movePos - (Vector2)this.transform.position;
            this.transform.Translate(_fBaseSpeed * Time.deltaTime * dir.normalized);
        }

        //To Idle
        if (_fDistanceToPlayer > _iDistanceToChase)
        {
            curState = State.Idle;
        }

        //To Attack
        if (_fDistanceToPlayer <= _iDistanceToAttack)
        {
            _tAttackSpeed = 0f;
            curState = State.Attack;
        }
    }

    protected override void Attack()
    {
        curState = State.Attack;

        _tAttackSpeed += Time.deltaTime;
        if (_tAttackSpeed >= _fAttackSpeed)
        {
            //animator.SetTrigger("Attack");
            animator.Play("Attack");
            _tAttackSpeed = 0f;
        }

        //To Chase
        if (_fDistanceToPlayer > _iDistanceToAttack)
        {
            curState = State.Chase;
        }
    }

    protected override void Hit(float damage)
    {
        curState = State.Hit;
        //animator.SetTrigger("Hit");
        animator.Play("Hit");
        this._fCurHP -= damage;
    }
}
