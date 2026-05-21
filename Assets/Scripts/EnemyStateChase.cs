public class EnemyStateChase : EnemyState
{
    private bool hasAlerted = false;

    public EnemyStateChase(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetAnim(Enemy.AnimState.Walk);
    }

    public override void Update(UnityEngine.Vector2 playerPos)
    {
        float dist = UnityEngine.Vector2.Distance(enemy.pos, playerPos);

        if (!hasAlerted)
        {
            hasAlerted = true;
            if (AudioManager.instance != null)
                AudioManager.instance.PlayEnemyAlert();
        }

        if (dist < enemy.attackRange && enemy.HasLineOfSight(playerPos))
            enemy.ChangeState(new EnemyStateAttack(enemy));
        else if (dist >= enemy.detectionRange || !enemy.HasLineOfSight(playerPos))
            enemy.ChangeState(new EnemyStateIdle(enemy));
        else
            enemy.MoveTowards(playerPos);
    }

    public override void Exit() { }
}