using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class BoarPatrolState : BaseState
{
    public override void onEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.CurrentSpeed = currentEnemy.WalkSpeed;
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwichState(NPCState.Chase);
        }
        Walk();
    }

    public override void PhysicsUpdate()
    {

    }
    public override void onExit()
    {

    }
    
    private void Walk()//一整套行走逻辑
    {
        bool shouldTurn = (!currentEnemy.pc.OnEdge) || (currentEnemy.pc.touchLeftWall && currentEnemy.FaceDir.x < 0) || (currentEnemy.pc.touchRightWall && currentEnemy.FaceDir.x > 0);
        if (!(currentEnemy.isHurt) && shouldTurn)
        {
            //转弯逻辑
            currentEnemy.transform.localScale = new Vector3(-currentEnemy.transform.localScale.x, currentEnemy.transform.localScale.y, currentEnemy.transform.localScale.z);
        }

        if (currentEnemy.pc.OnGround)
        {
            //是否行走的判断逻辑
            currentEnemy.walk();
            currentEnemy.ani.SetBool("isRun", false);
            currentEnemy.ani.SetBool("isWalk", true);
        }
        
    }
}