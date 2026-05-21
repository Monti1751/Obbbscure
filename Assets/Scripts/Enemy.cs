using UnityEngine;

public enum EnemyType { Doppleg, Vornie, Boss }

public class Enemy
{
    public Vector2 pos;
    public EnemyType type;
    public bool alive = true;

    public float health;
    public float detectionRange;
    public float attackRange;
    public float moveSpeed;
    public float attackCooldown;
    public int attackDamage;

    // Animación
    public enum AnimState { Idle, Walk, Attack, Die }
    public AnimState animState = AnimState.Idle;
    private float animTimer = 0f;
    private float animInterval = 0.15f;
    public int animFrame = 0;
    private bool deathAnimDone = false;

    // Máquina de estados
    private EnemyState currentState;

    public Enemy(Vector2 startPos, EnemyType enemyType = EnemyType.Doppleg)
    {
        pos = startPos;
        type = enemyType;

        float speedMultiplier = DifficultyManager.instance != null
            ? DifficultyManager.instance.GetEnemySpeed() / 1.5f
            : 1f;

        switch (type)
        {
            case EnemyType.Boss:
                health = 500f;
                detectionRange = 20f;
                attackRange = 8f;
                moveSpeed = 1.2f * speedMultiplier;
                attackCooldown = 3.0f;
                attackDamage = 15;
                break;
            case EnemyType.Vornie:
                health = 100f;
                detectionRange = 8f;
                attackRange = 1.5f;
                moveSpeed = 1.0f * speedMultiplier;
                attackCooldown = 2.0f;
                attackDamage = 20;
                break;
            default: // Doppleg
                health = 50f;
                detectionRange = 6f;
                attackRange = 1.5f;
                moveSpeed = 1.5f * speedMultiplier;
                attackCooldown = 1.5f;
                attackDamage = 10;
                break;
        }

        currentState = new EnemyStateIdle(this);
        currentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update(Vector2 playerPos)
    {
        if (GameOverManager.instance != null && GameOverManager.instance.IsGameEnded()) return;
        if (!alive)
        {
            UpdateDeathAnim();
            return;
        }

        currentState?.Update(playerPos);
        UpdateAnim();
    }

    public void FixedUpdate(Vector2 playerPos)
    {
        if (!alive) return;
        if (currentState is EnemyStateChase)
            MoveTowards(playerPos);
    }

    public void SetAnim(AnimState newAnim)
    {
        if (animState == newAnim) return;
        animState = newAnim;
        animFrame = 0;
        animTimer = 0f;
    }

    void UpdateAnim()
    {
        animTimer += Time.deltaTime;
        if (animTimer >= animInterval)
        {
            animTimer = 0f;
            int maxFrames = GetMaxFrames(animState);
            animFrame = (animFrame + 1) % maxFrames;
        }
    }

    public void UpdateDeathAnim()
    {
        if (deathAnimDone) return;
        animTimer += Time.deltaTime;
        if (animTimer >= animInterval)
        {
            animTimer = 0f;
            int maxFrames = GetMaxFrames(AnimState.Die);
            if (animFrame < maxFrames - 1)
                animFrame++;
            else
                deathAnimDone = true;
        }
    }

    int GetMaxFrames(AnimState anim)
    {
        switch (anim)
        {
            case AnimState.Walk: return 4;
            case AnimState.Attack: return 2;
            case AnimState.Die: return 3;
            default: return 1;
        }
    }

    public void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - pos).normalized;
        Vector2 newPos = pos + direction * moveSpeed * Time.fixedDeltaTime;
        if (!MapData.IsWall((int)newPos.x, (int)pos.y)) pos.x = newPos.x;
        if (!MapData.IsWall((int)pos.x, (int)newPos.y)) pos.y = newPos.y;
    }

    public bool HasLineOfSight(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - pos).normalized;
        float distance = Vector2.Distance(pos, targetPos);
        float step = 0.2f;

        for (float d = step; d < distance; d += step)
        {
            int checkX = (int)(pos.x + direction.x * d);
            int checkY = (int)(pos.y + direction.y * d);
            if (MapData.IsWall(checkX, checkY))
                return false;
        }
        return true;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            alive = false;
            ChangeState(new EnemyStateDead(this));
        }
    }
}