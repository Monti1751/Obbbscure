using UnityEngine;

public class Projectile
{
    public Vector2 pos;
    public Vector2 dir;
    public float speed = 6f;
    public bool active = true;
    public float damage = 10f;

    public Projectile(Vector2 startPos, Vector2 direction)
    {
        pos = startPos;
        dir = direction.normalized;
    }

    public void Update()
    {
        if (!active) return;

        Vector2 newPos = pos + dir * speed * Time.fixedDeltaTime;

        if (MapData.IsWall((int)newPos.x, (int)newPos.y))
        {
            active = false;
            return;
        }

        pos = newPos;

        // Comprobar colisión con jugador
        if (Raycaster.instance != null)
        {
            float dist = Vector2.Distance(pos, Raycaster.instance.pos);
            if (dist < 0.5f)
            {
                active = false;
                if (GameState.instance != null)
                    GameState.instance.TakeDamage((int)damage);
            }
        }
    }
}
