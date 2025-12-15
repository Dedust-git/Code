using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector]public Rigidbody2D rd;
    [HideInInspector]public Animator ani;
    [HideInInspector]public PhysicsCheck pc;
    [HideInInspector]public Buff buff;
    protected CapsuleCollider2D cc;
    [Header("检测")]
    public Vector2 centerFix;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;//攻击目标的层

    [Header("基础参数")]
    public float RunSpeed;
    public float WalkSpeed;
    public float HurtWait;//受伤硬值
    public float lostTime;//丢失目标时间
    [HideInInspector] public float CurrentSpeed;
     public float lostTimeCounter;
    [HideInInspector] public Vector3 FaceDir;
    [Header("基础状态")]
    public bool isHurt;
    public bool IsDead;
    private Coroutine hurtCoroutine; // 存储当前运行的协程

    protected BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;

    protected virtual void Awake()
    {
        buff = GetComponent<Buff>();
        patrolState = new BoarPatrolState();
        chaseState = new BoarChaseState();
        rd = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        pc = GetComponent<PhysicsCheck>();
        cc = GetComponent<CapsuleCollider2D>();
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.onEnter(this);
    }
    private void Update()
    {
        FaceDir = new Vector3(-transform.localScale.x, 0, 0);//调整面向
        int direction = (int)transform.localScale.x;//获取面向
        if (direction * (int)pc.EdgePositionFix.x > 0) pc.EdgePositionFix = new Vector2(pc.EdgePositionFix.x * -1, pc.EdgePositionFix.y);

        if (!pc.OnGround || IsDead || isHurt) GetComponent<Attack>().enabled = false;//不在地上不能攻击
        else GetComponent<Attack>().enabled = true;
        timeCounter();
    }

    private void FixedUpdate()
    {
        currentState.LogicUpdate();
    }
    private void OnDisable()
    {
        currentState.onExit();
    }
    public void timeCounter()
    {
        if (!FoundPlayer())
        {
            lostTimeCounter -= Time.deltaTime;
        } else
        {
            lostTimeCounter = lostTime;
        }
    }
    public virtual void walk()
    {
        if(!isHurt && !IsDead)rd.velocity = new Vector2(FaceDir.x * CurrentSpeed * Time.deltaTime, rd.velocity.y);
    }
    public bool FoundPlayer()
    {
        return Physics2D.BoxCast((Vector2)transform.position + centerFix, checkSize, 0, FaceDir, checkDistance, attackLayer);
    }
    public void SwichState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            _ => null
        };

        currentState.onExit();
        currentState = newState;
        currentState.onEnter(this);
    }
    #region 绘制范围
    private void OnDrawGizmosSelected()
    {//绘制范围
        Gizmos.DrawWireSphere(((Vector2)transform.position + centerFix) + new Vector2(checkDistance * -transform.localScale.x, 0), 0.2f);
    }
    #endregion
    #region 事件执行方法
    public virtual void TakeDamage(Attack attacker)
    {
        if(IsDead) return;
        ani.SetTrigger("Hurt");
        ani.SetBool("isHurt", true);

        isHurt = true;

        #region 受击面向敌人

        float direction = (int)transform.localScale.x;
        if (transform.localPosition.x - attacker.transform.localPosition.x < 0) direction = -1;
        else direction = 1;
        transform.localScale = new Vector3(direction, 1, 1);//受伤后面向玩家

        #endregion

        #region 判断攻击类型

        if (attacker.attackType == AttackType.SwardBash || attacker.attackType == AttackType.DownAttack)//判断攻击类型
        {
            buff.Knockup(attacker.force);
        }
        else if (attacker.attackType == AttackType.SwardAttack)
        {
            Vector2 BounceForceDir = (transform.position - attacker.transform.position).normalized;//击退方向
            rd.velocity = Vector3.zero;
            rd.AddForce(BounceForceDir * attacker.force, ForceMode2D.Impulse);//计算击退力度
        }
        
        #endregion

        if (hurtCoroutine != null) StopCoroutine(hurtCoroutine);
        hurtCoroutine = StartCoroutine(OnHurt());
    }
    IEnumerator OnHurt()
    {
        //Debug.Log($"TakeDamage called at {Time.time}");

        yield return new WaitForSeconds(HurtWait);

        isHurt = false;
        ani.SetBool("isHurt", false);
        hurtCoroutine = null;
    }

    public void OnDie()
    {
        ani.SetTrigger("dead");
        ani.SetBool("isHurt", false);
        IsDead = true;
        isHurt = false;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
    #endregion
}
