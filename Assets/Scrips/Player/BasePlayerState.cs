using UnityEngine;

public abstract class BasePlayerState:MonoBehaviour
{

    public abstract void onEnter();
    public abstract void onExit();
    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();
}
