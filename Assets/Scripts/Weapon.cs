using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Configuración")]
    public float fireRate = 0.5f;    // segundos entre disparos
    public int damage = 25;
    public float maxDistance = 10f;

    private float nextFireTime = 0f;

    void Update()
    {
        if (PauseMenu.instance != null && PauseMenu.instance.IsPaused()) return;
        if (GameOverManager.instance != null && GameOverManager.instance.IsGameEnded()) return;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }
    }

    void TryShoot()
    {
        if (Time.time < nextFireTime) return; // cadencia de disparo
        nextFireTime = Time.time + fireRate;

        // Restar munición
        if (GameState.instance.ammo <= 0)
        {
            //Debug.Log("¡Sin munición!");
            return;
        }
        GameState.instance.UseAmmo(1);

        Shoot();
    }

    void Shoot()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlayShoot();

        if (WeaponAnimator.instance != null)
            WeaponAnimator.instance.PlayFireAnimation();

        Vector2 playerPos = Raycaster.instance.pos;
        Vector2 playerDir = Raycaster.instance.dir;

        float distance = 0f;
        float step = 0.1f;

        while (distance < maxDistance)
        {
            distance += step;
            float checkX = playerPos.x + playerDir.x * distance;
            float checkY = playerPos.y + playerDir.y * distance;

            // ¿Golpeó un enemigo?
            foreach (var enemy in EnemyManager.instance.enemies)
            {
                if (!enemy.alive) continue;
                if (Vector2.Distance(new Vector2(checkX, checkY), enemy.pos) < 0.4f)
                {
                    enemy.TakeDamage(damage);
                    return;
                }
            }

            // ¿Golpeó una pared?
            if (MapData.IsWall((int)checkX, (int)checkY)) break;
        }
    }
}