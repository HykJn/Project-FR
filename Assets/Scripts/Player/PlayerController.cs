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
    public float attackSpeed;
    float _tickAttackSpeed;

    [Header("-- Status")]
    public int strength;
    public int intelligence;
    public float maxHP, curHP, maxMP, curMP;

    [Header("-- Skills")]
    public float defaultCoolDownReduction;
    public float skill_Q_CoolDown, skill_E_CoolDown, skill_R_CoolDown, skill_Shift_CoolDown, skill_Space_CoolDown;
    float _tickSkillQ, _tickSkillE, _tickSkillR, _tickSkillShift, _tickSkillSpace;
    [HideInInspector] public bool _flagInteractable, _flagSkillAvailable;
    public Skill skill_Q, skill_E, skill_R, skill_Shift, skill_Space;

    [Header("-- Weapon")]
    public bool swapable;
    public string pWeapon, sWeapon;
    public string curWeapon
    {
        get
        {
            return _flagSwap ? pWeapon : sWeapon;
        }
    }
    bool _flagSwap;
    
    float axisH, axisV;
    Vector2 inputDir, mousePos;
    Animator anim;

    public SpriteRenderer[] sprites;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {

    }

    private void Update()
    {
        GetInput();
        View();
        Move();
        Attack();
        SwapWeapon();

        //Skills
        if (_flagSkillAvailable)
        {
            Skill_Q();
            Skill_E();
            Skill_R();
            Skill_Shift();
            Skill_Space();
        }

    }

    public void Init()
    {
        //Components
        anim = GetComponentInChildren<Animator>();
        sprites = this.GetComponentsInChildren<SpriteRenderer>();

        //Status
        curHP = maxHP;
        curMP = maxMP;

        //Skills
        _tickSkillQ = skill_Q_CoolDown;
        _tickSkillE = skill_E_CoolDown;
        _tickSkillR = skill_R_CoolDown;
        _tickSkillShift = skill_Shift_CoolDown;
        _tickSkillSpace = skill_Space_CoolDown;
        _flagInteractable = false;
        _flagSkillAvailable = true;

        skill_Q = null;
        skill_E = null;
        skill_R = null;
        skill_Shift = null;
        skill_Space = null;

        //Swap
        _flagSwap = true;
    }

    public void GetInput()
    {
        axisH = Input.GetAxisRaw("Horizontal");
        axisV = Input.GetAxisRaw("Vertical");
        inputDir = new Vector2(axisH, axisV);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void View()
    {
        if (mousePos.x > this.transform.position.x)
        {
            this.GetComponent<RectTransform>().rotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            this.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
        }
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

        Vector2 moveDir = new Vector2(axisH, axisV).normalized;
        this.GetComponent<RectTransform>().position += baseSpeed * Time.deltaTime * (Vector3)moveDir;
    }

    public void Attack()
    {
        _tickAttackSpeed += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && _tickAttackSpeed >= attackSpeed)
        {
            Vector2 attackDir = (mousePos - (Vector2)this.transform.position).normalized;
            //GameObject slashObj = Instantiate(slashPrefab, ((Vector2)this.transform.position + attackDir*reach) + Vector2.up*0.3f, Quaternion.identity);
            GameObject slashObj = ObjectManager.instance.Activate("PlayerSlash");
            slashObj.transform.position = ((Vector2)this.transform.position + attackDir * reach) + Vector2.up * 0.3f;
            anim.SetTrigger("Attack");
            anim.SetFloat("AttackState", 0f);
            anim.SetFloat("NormalState", 0f);
            _tickAttackSpeed = 0f;
            print(curWeapon);
        }
    }

    public void Hit(float damage)
    {
        curHP -= damage;
    }

    public void Skill_Q()
    {
        _tickSkillQ += Time.deltaTime;
        if (_tickSkillQ >= skill_Q_CoolDown && Input.GetKeyDown(KeyCode.Q))
        {
            print("Q skill Activate");
            skill_Q.UseSkill(skill_Q.coType == CoefficientType.Physical ? strength : intelligence);
            _tickSkillQ = 0;
        }
    }

    public void Skill_E()
    {
        _tickSkillE += Time.deltaTime;
        if (!_flagInteractable && _tickSkillE >= skill_E_CoolDown && Input.GetKeyDown(KeyCode.E))
        {
            print("E skill Activate");
            if(skill_E == null)
            {
                Debug.Log("Not set skill on E");
            }
            _tickSkillE = 0;
        }
    }

    public void Skill_R()
    {
        _tickSkillR += Time.deltaTime;
        if (_tickSkillR >= skill_R_CoolDown && Input.GetKeyDown(KeyCode.R))
        {
            print("R skill Activate");
            _tickSkillR = 0;
        }
    }

    public void Skill_Shift()
    {
        _tickSkillShift += Time.deltaTime;
        if (_tickSkillShift >= skill_Shift_CoolDown && Input.GetKeyDown(KeyCode.LeftShift))
        {
            print("Shift skill Activate");
            _tickSkillShift = 0;
        }
    }

    public void Skill_Space()
    {
        _tickSkillSpace += Time.deltaTime;
        if (_tickSkillSpace >= skill_Space_CoolDown && Input.GetKeyDown(KeyCode.Space))
        {
            print("Space skill Activate");
            _tickSkillSpace = 0;
        }
    }


    public void SwapWeapon()
    {
        if (swapable && Input.GetKeyDown(KeyCode.Tab)) _flagSwap = !_flagSwap;
    }
    
    private void OnDrawGizmos()
    {
        if (axisH != 0)
        {
            Vector2 upper = this.transform.position + Vector3.up * 0.3f;
            Vector2 lower = this.transform.position + Vector3.down * 0.3f;

            Gizmos.DrawRay(upper, Vector3.right * axisH * 0.35f);
            Gizmos.DrawRay(lower, Vector3.right * axisH * 0.35f);

        }
        if (axisV != 0)
        {
            Vector2 left = this.transform.position + Vector3.left * 0.25f;
            Vector2 right = this.transform.position + Vector3.right * 0.25f;

            Gizmos.DrawRay(right, Vector3.up * axisV * 0.3f);
            Gizmos.DrawRay(left, Vector3.up * axisV * 0.3f);
        }
    }
    
}
