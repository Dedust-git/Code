public abstract class BaseState
{
    protected Enemy currentEnemy;
    public abstract void onEnter(Enemy enemy);
    public abstract void onExit();
    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();

}
