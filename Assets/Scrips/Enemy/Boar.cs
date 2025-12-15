using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Enemy
{
    // Start is called before the first frame update 
    public bool isRush;
    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoarPatrolState();
    }
    public override void walk()
    {
        if(!isHurt && !IsDead && !isRush)rd.velocity = new Vector2(FaceDir.x * CurrentSpeed * Time.deltaTime, rd.velocity.y);
    }
    public void RushAttack()
    {
        isRush = true;
        
    }
}
