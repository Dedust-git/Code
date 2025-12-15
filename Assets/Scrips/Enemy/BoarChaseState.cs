using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class BoarChaseState : BaseState
{
    public override void onEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.CurrentSpeed = currentEnemy.RunSpeed;
        currentEnemy.GetComponent<FloatEvent>().MakeText((Vector2)currentEnemy.transform.position,"!");
        if (Random.value <= 0.3f)
        {
            
        }
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <=0)
        {
            currentEnemy.SwichState(NPCState.Patrol);
        }
        Walk();
    }

    public override void PhysicsUpdate()
    {

    }
    public override void onExit()
    {
        if(!currentEnemy.IsDead)currentEnemy.GetComponent<FloatEvent>().MakeText((Vector2)currentEnemy.transform.position, "?");
        currentEnemy.ani.SetBool("isRun", false);
    }
    
    private void Walk()//一整套行走逻辑
    {
        bool shouldTurn = (!currentEnemy.pc.OnEdge) || (currentEnemy.pc.touchLeftWall && currentEnemy.FaceDir.x < 0) || (currentEnemy.pc.touchRightWall && currentEnemy.FaceDir.x > 0);
        if (!(currentEnemy.isHurt) && currentEnemy.pc.OnGround && shouldTurn)
        {
            //转弯逻辑
            currentEnemy.transform.localScale = new Vector3(-currentEnemy.transform.localScale.x, currentEnemy.transform.localScale.y, currentEnemy.transform.localScale.z);
        }

        if (currentEnemy.pc.OnGround)
        {
            //是否行走的判断逻辑
            currentEnemy.walk();
            currentEnemy.ani.SetBool("isRun", true);
            currentEnemy.ani.SetBool("isWalk", false);
        }
    }
}