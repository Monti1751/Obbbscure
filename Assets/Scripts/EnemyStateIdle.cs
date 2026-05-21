public class EnemyStateIdle : EnemyState
{
    public EnemyStateIdle(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetAnim(Enemy.AnimState.Idle);
    }

    public override void Update(UnityEngine.Vector2 playerPos)
    {
        float dist = UnityEngine.Vector2.Distance(enemy.pos, playerPos);

        if (dist < enemy.attackRange && enemy.HasLineOfSight(playerPos))
            enemy.ChangeState(new EnemyStateAttack(enemy));
        else if (dist < enemy.detectionRange && enemy.HasLineOfSight(playerPos))
            enemy.ChangeState(new EnemyStateChase(enemy));
    }

    public override void Exit() { }
}