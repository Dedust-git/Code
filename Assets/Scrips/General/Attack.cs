using System.Collections;
using UnityEngine;
using UnityEngine.Events;
public class Attack : MonoBehaviour
{
    [Header("基础属性")]
    public int currentDamage;
    public int DefaltDamage;
    public float force;//攻击的力道
    public AttackType attackType;
    [Header("攻击特效")]
    public bool enableAttackEffort = false;//是否开启挥动效果，默认否
    public bool enableHitEffort = false;//是否开启命中效果，默认否
    public float ShakeForce;//摇晃幅度
    
    #region 事件注册
    public UnityEvent cameraShake;
    #endregion
    private void Start()//不可删除，删除后不可关闭，很奇怪但我不知道为什么
    {

    }
    private void Awake()
    {
        currentDamage = DefaltDamage;
    }
    private void OnEnable()
    {
        if(!enableAttackEffort) return;//没有发生特效则跳过 
        cameraShake?.Invoke();
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (attackType != AttackType.CollisionAttack) return;//碰撞攻击是持续检查的
        other.GetComponent<Characters>()?.TakeDamage(this);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attackType == AttackType.CollisionAttack) return;//如果是非碰撞攻击，只取发生状态
        other.GetComponent<Characters>()?.TakeDamage(this);

        if (attackType == AttackType.DownAttack)
        {
            other.GetComponent<FragileBlock>()?.OnBreak();
        }
    
    }
}
