using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Buff : MonoBehaviour
{
    private Rigidbody2D rd;
    private PhysicsCheck pc;

    private void Awake()
    {
        rd = GetComponent<Rigidbody2D>();
        pc = GetComponent<PhysicsCheck>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }
    private void FixedUpdate()
    {
        InvincibleCulculator();
    }
    private void Update()
    {
        if (watingForKnockup && pc.OnGround)
        {
            Knockup(Knockupforce);
        }
    }
    #region 击飞
    private bool watingForKnockup;//隐藏状态：击飞  
    private float Knockupforce;
    public void Knockup(float force)
    {
        if (pc.OnGround)
        {
            rd.velocity = Vector3.zero;
            rd.AddForce(Vector2.up *force, ForceMode2D.Impulse);//计算击退力度
            watingForKnockup = false;
            Knockupforce = 0; 
        }
        else
        {
            rd.velocity = Vector3.zero;
            watingForKnockup = true;
            Knockupforce = force;
            rd.AddForce(Vector2.down * 1.5f * force, ForceMode2D.Impulse);//计算击退力度
        }
    }
    #endregion

    #region 无敌
    [Header("无敌状态")]
    public bool IsInvincibility;//是否处于无敌状态
    [SerializeField]
    private int InvincibilityFrame;//无敌帧计时器
    [SerializeField]
    private int InvincibilityCounter;//无敌帧计时器
    public void OnInvincible()//发生无敌
    {
        InvincibilityCounter = InvincibilityFrame;
        IsInvincibility = true;
    }
    private SpriteRenderer spriteRenderer; // 用于控制透明度
    private Color originalColor;           // 存储原始颜色
    private void InvincibleCulculator()
    {
        if (IsInvincibility)
        {
            InvincibilityCounter--;
            if (InvincibilityCounter <= 0)
            {
                IsInvincibility = false;
                spriteRenderer.color = originalColor;
                return;
            }

            bool ShouldBlink = (InvincibilityCounter % 3) == 0;
            if (ShouldBlink)
            {
                spriteRenderer.color = (InvincibilityCounter & 1) == 0 ?
                new Color(originalColor.r, originalColor.g, originalColor.b, 0.1f) : originalColor;
            }
        }
    }
    #endregion

}
