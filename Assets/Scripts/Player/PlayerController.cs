using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("- Movement")]
    public float baseSpeed;

    [Header("-- Attack")]
    public GameObject slashPrefab;
    public float reach;

    float axisH, axisV;
    Vector2 inputDir, mousePos;
    Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        GetInput();
        Move();
        Attack();
    }

    public void GetInput()
    {
        axisH = Input.GetAxisRaw("Horizontal");
        axisV = Input.GetAxisRaw("Vertical");
        inputDir = new Vector2(axisH, axisV);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void Move()
    {
        if (inputDir.magnitude > 0)
        {
            anim.SetBool("Run", true);
            anim.SetFloat("RunState", 0.5f);
        }
        else
        {
            anim.SetBool("Run", false);
            anim.SetFloat("RunState", 0f);
        }


        if (axisH > 0)
        {
            this.GetComponent<RectTransform>().rotation = new Quaternion(0, 180, 0, 0);
        }
        else if (axisH < 0)
        {
            this.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
        }

        int wallLayer = 1 << 3;
        if (axisH != 0)
        {
            Vector2 upper = this.transform.position + Vector3.up * 0.3f;
            Vector2 lower = this.transform.position + Vector3.down * 0.3f;

            if (Physics2D.Raycast(upper, Vector3.right * axisH, 0.275f, wallLayer) ||
                Physics2D.Raycast(lower, Vector3.right * axisH, 0.275f, wallLayer))
            {
                axisH = 0;
            }
        }
        if (axisV != 0)
        {
            Vector2 left = this.transform.position + Vector3.left * 0.25f;
            Vector2 right = this.transform.position + Vector3.right * 0.25f;

            if (Physics2D.Raycast(right, Vector3.up * axisV, 0.325f, wallLayer) ||
                Physics2D.Raycast(left, Vector3.up * axisV, 0.325f, wallLayer))
            {
                axisV = 0;
            }
        }

        Vector2 moveDir = new Vector2(-Mathf.Abs(axisH), axisV).normalized;
        this.transform.Translate(baseSpeed * Time.deltaTime * moveDir);
    }

    public void Attack()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 attackDir = (mousePos - (Vector2)this.transform.position).normalized;
            GameObject slashObj = Instantiate(slashPrefab, (Vector2)this.transform.position + attackDir*reach, Quaternion.identity);
            anim.SetTrigger("Attack");
            anim.SetFloat("AttackState", 0f);
            anim.SetFloat("NormalState", 0f);
        }
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
