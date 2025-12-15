using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    // Start is called before the first frame update
    
    [Header("检查参数")]
    public float CheckRange;//碰撞检测范围
    public LayerMask GroundLayer;//定义一个碰撞处理，表示区检查什么东西
    public Vector2 EdgePositionFix;//边缘位置修正

    public Vector2 GroundPositionFix;//地面位置修正
    public Vector2 LeftPosionFix;//左侧位置修正
    public Vector2 RightPosionFix;//右侧位置修正
    [Header("实时参数")]
    public bool OnEdge;//是否在悬崖边上
    public bool OnGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    [Header("材质")]
    [SerializeField] public PhysicsMaterial2D normalMaterial; 
    [SerializeField] public PhysicsMaterial2D noFriction; 
    // Update is called once per frame
    void Update()
    {
        Check();
    }

    private void Check()//检查
    {
        OnGround = Physics2D.OverlapCircle((Vector2)transform.position + GroundPositionFix, CheckRange, GroundLayer);

        OnEdge = Physics2D.OverlapCircle((Vector2)transform.position + EdgePositionFix, CheckRange, GroundLayer);
        //墙体判断
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + LeftPosionFix, CheckRange, GroundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + RightPosionFix, CheckRange, GroundLayer);
    }

    private void OnDrawGizmosSelected()
    {//绘制范围
        Gizmos.DrawWireSphere((Vector2)transform.position + EdgePositionFix, CheckRange);
        Gizmos.DrawWireSphere((Vector2)transform.position + GroundPositionFix, CheckRange);
        Gizmos.DrawWireSphere((Vector2)transform.position + LeftPosionFix, CheckRange);
        Gizmos.DrawWireSphere((Vector2)transform.position + RightPosionFix, CheckRange);
    }
}
