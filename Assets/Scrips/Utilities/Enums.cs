public enum NPCState//NPC状态标签
{
    Patrol,
    Chase,
    Skill
}
public enum AttackType//攻击类型标签
{
    DownAttack, 
    SwardAttack, 
    CollisionAttack, 
    SwardBash
}
public enum GroundTypeEnums//地面标签
{
    grass,
    dirt,
    defaultType
}
public enum CreatureType//生物标签
{
    Player,
    Monster
}
public enum PlayerState {//玩家状态标签
    Idle,       // 待机
    Running,    // 移动/奔跑
    Jumping,    // 跳跃/空中
    Attacking,  // 普通攻击
    DownAttack, // 下劈
    Hurt,       // 受击
    Dead        // 死亡
}
