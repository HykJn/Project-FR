using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public enum State
    {
        Idle, Chase, Attack, Hit, None
    }

    [Header("-- General")]
    public State curState = State.Idle;
    public float _fMaxHP, _fCurHP, _fBaseSpeed, _fDistanceToChase, _fDistanceToAttack, _fAttackSpeed;

    protected float _fDistanceToPlayer, _tAttackSpeed;
    protected Animator animator;

    protected virtual void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        _fDistanceToPlayer = Vector2.Distance(this.transform.position, GameObject.FindWithTag("Player").transform.position);
        ChangeState(curState);
    }

    public void ChangeState(State state)
    {
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    protected abstract void Idle();
    protected abstract void Chase();
    protected abstract void Attack();
    protected abstract void Hit(float damage);
}
