using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("- Movement")]
    public float baseSpeed;

    float axisH, axisV;
    Vector2 inputDir;
    SpriteRenderer render;
    Animator anim;

    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        GetInput();
        Move();
    }

    public void GetInput()
    {
        axisH = Input.GetAxisRaw("Horizontal");
        axisV = Input.GetAxisRaw("Vertical");
        inputDir = new Vector2(axisH, axisV);
    }

    public void Move()
    {
        if (inputDir.magnitude > 0) anim.SetBool("Move", true);
        else anim.SetBool("Move", false);

        if (axisH > 0) render.flipX = false;
        else if (axisH < 0) render.flipX = true;

        int wallLayer = 1 << 3;
        if(axisH != 0)
        {
            Vector2 upper = this.transform.position + Vector3.up * 0.3f;
            Vector2 lower = this.transform.position + Vector3.down * 0.3f;

            if(Physics2D.Raycast(upper, Vector3.right * axisH, 0.275f, wallLayer) ||
                Physics2D.Raycast(lower, Vector3.right * axisH, 0.275f, wallLayer))
            {
                axisH = 0;
            }
        }
        if(axisV != 0)
        {
            Vector2 left = this.transform.position + Vector3.left * 0.25f;
            Vector2 right = this.transform.position + Vector3.right * 0.25f;

            if(Physics2D.Raycast(right, Vector3.up * axisV, 0.325f, wallLayer) ||
                Physics2D.Raycast(left, Vector3.up * axisV, 0.325f, wallLayer))
            {
                axisV = 0;
            }
        }

        Vector2 moveDir = new Vector2(axisH, axisV).normalized;
        this.transform.Translate(baseSpeed * Time.deltaTime * moveDir);
    }

    /*
    private void OnDrawGizmos()
    {
        if (axisH != 0)
        {
            Vector2 upper = this.transform.position + Vector3.up * 0.3f;
            Vector2 lower = this.transform.position + Vector3.down * 0.3f;

            Gizmos.DrawRay(upper, Vector3.right * axisH * 0.25f);
            Gizmos.DrawRay(lower, Vector3.right * axisH * 0.25f);

        }
        if (axisV != 0)
        {
            Vector2 left = this.transform.position + Vector3.left * 0.25f;
            Vector2 right = this.transform.position + Vector3.right * 0.25f;

            Gizmos.DrawRay(right, Vector3.up * axisV * 0.3f);
            Gizmos.DrawRay(left, Vector3.up * axisV * 0.3f);
        }
    }
    */
}
