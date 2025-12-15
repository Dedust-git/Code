using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class PlayerController : MonoBehaviour
{
    #region 引用
    private Rigidbody2D rb;//调用Rigidbody
    private PhysicsCheck pc;
    private PlayerInputControl inputControl;
    private PlayerAnimation pa;
    private CapsuleCollider2D cc;
    private Characters cct;
    public Vector2 inputDirection;

    private Attack attack3;
    #endregion
    [Header("基础属性")]
    public float speed;
    public float walkspeed;
    public float jumplevel;
    public int DownAttackCost;
    [Header("状态")]
    public bool isRunning;
    public bool isDownAttack;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    [Header("调试")]
    [SerializeField]
    private bool manualJump;

    public UnityEvent ShakeGround;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();//获取信息
        pc = GetComponent<PhysicsCheck>();
        pa = GetComponent<PlayerAnimation>();
        cc = GetComponent<CapsuleCollider2D>();
        cct = GetComponent<Characters>();
        attack3 = transform.Find("AttackRange/attack3").GetComponent<Attack>();
        inputControl = new PlayerInputControl();

        inputControl.Player.Jump.started += Jump;//注册一个函数
        inputControl.Player.OncombSJ.started += DownAttack;//注册下劈
        inputControl.Player.Attack.started += Attack;//注册attack
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {
        inputDirection = inputControl.Player.Move.ReadValue<Vector2>();//读取速度
        isRunning = inputControl.Player.Run.ReadValue<float>() != 0 ? false : true;//shift控制是否奔跑

        // if (isDownAttack) GetComponent<CapsuleCollider2D>().enabled = false;
        // else  GetComponent<CapsuleCollider2D>().enabled = true;
        if (pc.OnGround)//在地面上存在摩擦力，天上不存在摩擦力防止粘墙上了
        {
            if (isDownAttack)
            {
                isDownAttack = false;//在地上取消跳劈
                ShakeGround?.Invoke();
            }
            cc.sharedMaterial = pc.normalMaterial;

            attack3.force = 10;
            attack3.enableAttackEffort=true;
        }
        else
        {
            cc.sharedMaterial = pc.noFriction;

            attack3.force = 15;
            attack3.enableAttackEffort=false;
        }
    }

    private void FixedUpdate()//持续更新
    {
        if ((!isAttack && pc.OnGround) || !pc.OnGround) Move();
    }

    private void Move()
    {
        if(isDownAttack) return;//下劈期间不能移动
        if (isRunning) rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        else rb.velocity = new Vector2(inputDirection.x * walkspeed * Time.deltaTime, rb.velocity.y);

        int Direction = (int)transform.localScale.x;//真实方向
        if (inputDirection.x > 0) Direction = 1;
        if (inputDirection.x < 0) Direction = -1;

        transform.localScale = new Vector3(Direction, 1, 1);
    }


    private void Jump(InputAction.CallbackContext context)
    {
        //Debug.Log("jump");
        if (pc.OnGround)
        {
            if (isAttack)
            {
                isAttack = false;//打断攻击
                pa.JumpBrake();//跳跃打断攻击
            }
            rb.AddForce(transform.up * jumplevel, ForceMode2D.Impulse);
        }
            
    }
    private void DownAttack(InputAction.CallbackContext context)
    {
        if (pc.OnGround || isDownAttack || !cct.OnCostPower(AttackType.DownAttack) ) return;

        isAttack = false;
        isDownAttack = true;
        pa.PlayDownAttack();//执行动画

        rb.velocity = 20.0f * Vector2.down;
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (isDownAttack)
        {
            isAttack = false;
            return;
        }

        isAttack = true;

        pa.PlayAttack();
    }


    #region UnityEvent
    public void PlayHurt(Attack attacker)
    {
        //Debug.Log("Successful");
        isHurt = true;
        Vector2 ForceDirection = new Vector2(transform.position.x - attacker.transform.position.x, 1.0f).normalized;  
        rb.AddForce(ForceDirection * attacker.force, ForceMode2D.Impulse);
        //rb.velocity = ForceDirection * knockBackForce;
    }

    public void PlayDead()
    {
        isDead = true;
        inputControl.Disable();
    }
    #endregion
}