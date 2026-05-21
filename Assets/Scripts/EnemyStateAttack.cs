public class EnemyStateAttack : EnemyState
{
    private float nextAttackTime = 0f;

    public EnemyStateAttack(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetAnim(Enemy.AnimState.Attack);
        nextAttackTime = UnityEngine.Time.time + enemy.attackCooldown;
    }

    public override void Update(UnityEngine.Vector2 playerPos)
    {
        float dist = UnityEngine.Vector2.Distance(enemy.pos, playerPos);

        if (dist >= enemy.attackRange || !enemy.HasLineOfSight(playerPos))
        {
            enemy.ChangeState(new EnemyStateChase(enemy));
            return;
        }

        if (UnityEngine.Time.time >= nextAttackTime)
        {
            nextAttackTime = UnityEngine.Time.time + enemy.attackCooldown;

            if (enemy.type == EnemyType.Boss)
            {
                if (ProjectileManager.instance != null)
                {
                    var dir = (playerPos - enemy.pos).normalized;
                    ProjectileManager.instance.SpawnProjectile(enemy.pos, dir);
                }
            }
            else
            {
                GameState.instance.TakeDamage(enemy.attackDamage);
            }
        }
    }

    public override void Exit() { }
}