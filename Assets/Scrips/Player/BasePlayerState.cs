using UnityEngine;

public abstract class BasePlayerState:MonoBehaviour
{
    //暂时没什么用
    public abstract void onEnter();
    public abstract void onExit();
    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();
}
