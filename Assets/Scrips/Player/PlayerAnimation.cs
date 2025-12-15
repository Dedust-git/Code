using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public enum AnimationLayer
    {
        BaseLayer = 0,     // 基础层
        AttackLayer = 1,   // 攻击层
        HurtLayer = 2 // 上半身层
    }
    private Animator ani;
    private Rigidbody2D rd;
    private PlayerController pc;
    private PhysicsCheck phc;
    void Awake()
    {
        ani = GetComponent<Animator>();
        rd = GetComponent<Rigidbody2D>();
        pc = GetComponent<PlayerController>();
        phc = GetComponent<PhysicsCheck>();
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimation();
    }

    void SetAnimation()
    {
        ani.SetFloat("velocityX", math.abs(rd.velocity.x));
        ani.SetFloat("velocityY", rd.velocity.y);
        ani.SetBool("isRunning", pc.isRunning);
        ani.SetBool("onGround", phc.OnGround);
        ani.SetBool("isDead", pc.isDead);
        ani.SetBool("isAttack", pc.isAttack);
        if (phc.OnGround) ani.SetBool("isdownattack", false); 
        // Debug.Log("Successfully");
    }

    public void JumpBrake()//跳跃打断动画
    {
        ani.Play("attackbase", 1, 0f);
    }
    public void PlayHurt(Attack attacker)
    {
        //Debug.Log("Successful");
        ani.SetTrigger("hurt");
    }

    public void PlayAttack()
    {
        //Debug.Log("Successful");
        ani.SetTrigger("Attack");
    }
    
    public void PlayDownAttack()
    {
        //Debug.Log("Successful");
        ani.SetTrigger("downattack");
        ani.SetBool("isdownattack",true);
    }
}
