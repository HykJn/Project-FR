using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
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
        if (collision.CompareTag("PlayerAttack"))
        {
            Hit(1);
        }
    }

    protected override void Idle()
    {
        curState = State.Idle;
        animator.SetTrigger("Idle");

        //To Chase
        if (_fDistanceToPlayer <= _fDistanceToChase)
        {
            curState = State.Chase;
        }
    }

    protected override void Chase()
    {
        curState = State.Chase;
        animator.SetTrigger("Chase");
        Vector2 moveDir = GameObject.FindWithTag("Player").transform.position - this.transform.position;
        this.transform.Translate(_fBaseSpeed * Time.deltaTime * moveDir.normalized);

        //To Idle
        if (_fDistanceToPlayer > _fDistanceToChase)
        {
            curState = State.Idle;
        }

        //To Attack
        if (_fDistanceToPlayer <= _fDistanceToAttack)
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
            animator.SetTrigger("Attack");
            _tAttackSpeed = 0f;
        }

        //To Chase
        if (_fDistanceToPlayer > _fDistanceToAttack)
        {
            curState = State.Chase;
        }
    }

    protected override void Hit(float damage)
    {
        curState = State.Hit;
        animator.SetTrigger("Hit");
        this._fCurHP -= damage;
    }
}
