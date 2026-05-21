public class EnemyStateDead : EnemyState
{
    public EnemyStateDead(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animState = Enemy.AnimState.Die;
        enemy.animFrame = 0;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayEnemyDie();
        if (ScoreManager.instance != null)
            ScoreManager.instance.AddKillPoints();

        if (enemy.type == EnemyType.Boss)
            if (GameOverManager.instance != null)
                GameOverManager.instance.Invoke("TriggerVictory", 3f);
    }

    public override void Update(UnityEngine.Vector2 playerPos)
    {
        enemy.UpdateDeathAnim();
    }

    public override void Exit() { }
}