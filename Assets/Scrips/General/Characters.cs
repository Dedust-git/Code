using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Composites;
using UnityEngine.TextCore.Text;

public class Characters : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;//调用Rigidbody
    private Buff buff;
    [Header("基础属性")]
    public float particleForce;//制造粒子的力度
    public int MaxHealth; //最大生命
    public int MaxPower; //最大能量
    public float PowerRecoveringSpeed;//下劈回复速率
    public float PowerRecoverTime;//下劈后所需回复时间
    public int DownAttackCost;//下劈消耗能量
    public CreatureType creatureType;
    [Header("实时属性")]
    public int CurrentHealth;
    public int CurrentPower;

    #region 计时器
    private float PowerRecoverCounter;
    private float powerRecoverAccumulator;
    #endregion 

    #region 事件注册
    [Header("属性变化")]
    public UnityEvent<Characters> OnCharacterChanged;
    [Header("受伤")] 
    public UnityEvent<Attack> OnTakeDamage;
    public UnityEvent CameraShake;
    public UnityEvent OnDead;
    #endregion
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        buff = GetComponent<Buff>();

        CurrentHealth = MaxHealth;
        CurrentPower = MaxPower;

        OnCharacterChanged?.Invoke(this);
    }
    private void Update()
    {
        RecoverPower();
    }

    private void FixedUpdate()
    {
       
    }
    #region 能量逻辑
    ///<summary>
    /// 接受能量消耗值并返回是否可以消耗
    ///</summary>
    ///<param name="CostAmount">消耗值（整数）</param> 
    public Boolean OnCostPower(int CostAmount)
    {
        if (CurrentPower >= CostAmount)
        {
            if (CostAmount == 0) return true;//0直接返回
            PowerRecoverCounter = PowerRecoverTime;//设置回复时间
            CurrentPower -= CostAmount;
            OnCharacterChanged?.Invoke(this);
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 根据攻击类型自动判断能量消耗
    /// </summary>
    /// <param name="attackType">攻击类型枚举</param>
    public Boolean OnCostPower(AttackType attackType)
    {
        //Debug.Log("成功接受判断");
        int CostAmount = attackType switch
        {
            AttackType.DownAttack => DownAttackCost,
            _ => 0
        };
        if (CurrentPower >= CostAmount)
        {
            if (CostAmount == 0) return true;//0直接返回
            PowerRecoverCounter = PowerRecoverTime;//设置回复时间
            CurrentPower -= CostAmount;
            OnCharacterChanged?.Invoke(this);
            return true;
        }
        else
        {
            //Debug.Log("当前能量为" + CurrentPower + "消耗值为" + CostAmount);
            return false;
        }
    }
    private void RecoverPower()
    {
        if (PowerRecoverCounter <= 0)
        {
            if (CurrentPower >= MaxPower) return; // 能量已满，不需要恢复

            // 累积恢复量
            powerRecoverAccumulator += PowerRecoveringSpeed * Time.deltaTime;

            // 当累积量达到1点能量时，进行恢复
            if (powerRecoverAccumulator >= 10f)
            {
                int recoverAmount = Mathf.FloorToInt(powerRecoverAccumulator);//能量向下取整
                CurrentPower = Mathf.Min(CurrentPower + recoverAmount, MaxPower);//不能超过最大血量
                powerRecoverAccumulator -= recoverAmount; // 保留小数部分

                // 触发能量改变事件
                OnCharacterChanged?.Invoke(this);
            }
            if (CurrentPower == MaxPower) powerRecoverAccumulator = 0;//满能量后清空积累的能量
        }
        else
        {
            PowerRecoverCounter -= Time.deltaTime;
        }
    }
    #endregion

    public void TakeDamage(Attack attacker)
    {
        if (buff.IsInvincibility) return;//如果还在无敌状态，跳过检查     
        if (creatureType == CreatureType.Player) buff.OnInvincible();//如果是玩家，来个无敌帧
        if(attacker.enableHitEffort) CameraShake?.Invoke();//如果对方有命中特效，命中时摇晃下屏幕

        OnTakeDamage?.Invoke(attacker);
        CurrentHealth -= attacker.currentDamage;//受伤

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnDead?.Invoke();
            return;
        }

        OnCharacterChanged?.Invoke(this);
        //受伤事件
    }

}
