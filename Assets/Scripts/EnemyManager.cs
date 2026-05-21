using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<Enemy> enemies = new List<Enemy>();

    [Header("Sprites Doppleg")]
    public Texture2D enemy1_idle;
    public Texture2D[] enemy1_walk;
    public Texture2D[] enemy1_attack;
    public Texture2D[] enemy1_die;

    [Header("Sprites Vornie")]
    public Texture2D enemy2_idle;
    public Texture2D[] enemy2_walk;
    public Texture2D[] enemy2_attack;
    public Texture2D[] enemy2_die;

    [Header("Sprites Boss")]
    public Texture2D boss_idle;
    public Texture2D[] boss_walk;
    public Texture2D[] boss_attack;
    public Texture2D[] boss_die;

    void Awake() { instance = this; }

    void Start()
    {
        SpawnEnemiesForLevel(MapData.currentLevel);
    }

    void AddEnemy(float x, float y, EnemyType type)
    {
        if (MapData.GetWallType((int)x, (int)y) == 0)
            enemies.Add(new Enemy(new Vector2(x, y), type));
        /*else(
            Debug.LogWarning($"Spawn inválido ({x},{y}) tipo={MapData.GetWallType((int)x, (int)y)}"
            );*/
	}
    public void SpawnEnemiesForLevel(int level)
    {
        enemies.Clear();
        switch (level)
        {
            case 1:
                AddEnemy(11.5f, 3.5f, EnemyType.Doppleg);
                AddEnemy(20.5f, 3.5f, EnemyType.Doppleg);
                AddEnemy(11.5f, 8.5f, EnemyType.Doppleg);
                AddEnemy(15.5f, 8.5f, EnemyType.Vornie);
                AddEnemy(4.5f, 10.5f, EnemyType.Doppleg);
                AddEnemy(4.5f, 11.5f, EnemyType.Doppleg);
                AddEnemy(18.5f, 10.5f, EnemyType.Vornie);
                AddEnemy(19.5f, 11.5f, EnemyType.Doppleg);
                AddEnemy(3.5f, 16.5f, EnemyType.Vornie);
                AddEnemy(10.5f, 19.5f, EnemyType.Doppleg);
                AddEnemy(18.5f, 19.5f, EnemyType.Vornie);
                AddEnemy(20.5f, 15.5f, EnemyType.Doppleg);
                AddEnemy(20.5f, 21.5f, EnemyType.Vornie);
                break;
            case 2:
                AddEnemy(12.5f, 3.5f, EnemyType.Doppleg);
                AddEnemy(20.5f, 3.5f, EnemyType.Vornie);
                AddEnemy(25.5f, 3.5f, EnemyType.Doppleg);
                AddEnemy(5.5f, 10.5f, EnemyType.Vornie);
                AddEnemy(13.5f, 10.5f, EnemyType.Doppleg);
                AddEnemy(20.5f, 10.5f, EnemyType.Vornie);
                AddEnemy(3.5f, 18.5f, EnemyType.Doppleg);
                AddEnemy(10.5f, 20.5f, EnemyType.Vornie);
                AddEnemy(18.5f, 20.5f, EnemyType.Doppleg);
                AddEnemy(24.5f, 20.5f, EnemyType.Vornie);
                break;
            case 3:
                AddEnemy(10.5f, 3.5f, EnemyType.Vornie);
                AddEnemy(18.5f, 3.5f, EnemyType.Vornie);
                AddEnemy(26.5f, 3.5f, EnemyType.Vornie);
                AddEnemy(29.5f, 3.5f, EnemyType.Vornie);
                AddEnemy(5.5f, 10.5f, EnemyType.Doppleg);
                AddEnemy(12.5f, 10.5f, EnemyType.Vornie);
                AddEnemy(20.5f, 10.5f, EnemyType.Doppleg);
                AddEnemy(28.5f, 10.5f, EnemyType.Vornie);
                AddEnemy(3.5f, 18.5f, EnemyType.Doppleg);
                AddEnemy(10.5f, 18.5f, EnemyType.Vornie);
                AddEnemy(18.5f, 18.5f, EnemyType.Doppleg);
                AddEnemy(26.5f, 18.5f, EnemyType.Vornie);
                AddEnemy(15.5f, 21.5f, EnemyType.Vornie);
                break;
            case 4:
                AddEnemy(11.5f, 3.5f, EnemyType.Doppleg);
                AddEnemy(20.5f, 3.5f, EnemyType.Vornie);
                AddEnemy(7.5f, 8.5f, EnemyType.Doppleg);
                AddEnemy(15.5f, 8.5f, EnemyType.Vornie);
                AddEnemy(7.5f, 10.5f, EnemyType.Vornie);
                AddEnemy(21.5f, 10.5f, EnemyType.Doppleg);
                AddEnemy(3.5f, 16.5f, EnemyType.Vornie);
                AddEnemy(10.5f, 19.5f, EnemyType.Doppleg);
                AddEnemy(21.5f, 19.5f, EnemyType.Vornie);
                AddEnemy(21.5f, 21.5f, EnemyType.Vornie);
                break;
            case 5:
                AddEnemy(11.5f, 9.5f, EnemyType.Boss);
                break;
        }
    }

    Texture2D GetEnemySprite(Enemy enemy)
    {
        bool isDoppleg = enemy.type == EnemyType.Doppleg;
        bool isBoss = enemy.type == EnemyType.Boss;

        Texture2D[] walkFrames = isBoss ? boss_walk : isDoppleg ? enemy1_walk : enemy2_walk;
        Texture2D[] attackFrames = isBoss ? boss_attack : isDoppleg ? enemy1_attack : enemy2_attack;
        Texture2D[] dieFrames = isBoss ? boss_die : isDoppleg ? enemy1_die : enemy2_die;
        Texture2D idleFrame = isBoss ? boss_idle : isDoppleg ? enemy1_idle : enemy2_idle;

        switch (enemy.animState)
        {
            case Enemy.AnimState.Walk:
                if (walkFrames != null && walkFrames.Length > 0)
                    return walkFrames[enemy.animFrame % walkFrames.Length];
                break;
            case Enemy.AnimState.Attack:
                if (attackFrames != null && attackFrames.Length > 0)
                    return attackFrames[enemy.animFrame % attackFrames.Length];
                break;
            case Enemy.AnimState.Die:
                if (dieFrames != null && dieFrames.Length > 0)
                    return dieFrames[Mathf.Clamp(enemy.animFrame, 0, dieFrames.Length - 1)];
                break;
        }

        return idleFrame;
    }

    void Update()
    {
        if (Raycaster.instance == null) return;
        Vector2 playerPos = Raycaster.instance.pos;
        foreach (var enemy in enemies)
            enemy.Update(playerPos);
    }

    void FixedUpdate()
    {
        if (Raycaster.instance == null) return;
        Vector2 playerPos = Raycaster.instance.pos;
        foreach (var enemy in enemies)
            enemy.FixedUpdate(playerPos);
    }

    public void DrawEnemy(Enemy enemy, Color32[] pixels, int screenWidth, int screenHeight,
                          Vector2 playerPos, Vector2 playerDir, Vector2 playerPlane, float[] zBuffer)
    {
        Texture2D sprite = GetEnemySprite(enemy);
        if (sprite == null) return;

        Vector2 relPos = enemy.pos - playerPos;
        float invDet = 1f / (playerPlane.x * playerDir.y - playerDir.x * playerPlane.y);
        float transformX = invDet * (playerDir.y * relPos.x - playerDir.x * relPos.y);
        float transformY = invDet * (-playerPlane.y * relPos.x + playerPlane.x * relPos.y);
        if (transformY <= 0) return;

        int spriteScreenX = (int)((screenWidth / 2) * (1 + transformX / transformY));
        int spriteHeight = Mathf.Abs((int)(screenHeight / transformY));
        int spriteWidth = spriteHeight;

        int drawStartY = Mathf.Max(0, screenHeight / 2 - spriteHeight / 2);
        int drawEndY = Mathf.Min(screenHeight - 1, screenHeight / 2 + spriteHeight / 2);
        int drawStartX = Mathf.Max(0, spriteScreenX - spriteWidth / 2);
        int drawEndX = Mathf.Min(screenWidth - 1, spriteScreenX + spriteWidth / 2);

        for (int x = drawStartX; x < drawEndX; x++)
        {
            if (transformY >= zBuffer[x]) continue;
            int texX = (int)((x - (spriteScreenX - spriteWidth / 2)) * sprite.width / spriteWidth);
            texX = Mathf.Clamp(texX, 0, sprite.width - 1);

            for (int y = drawStartY; y < drawEndY; y++)
            {
                int texY = (int)((y - (screenHeight / 2 - spriteHeight / 2)) * sprite.height / spriteHeight);
                texY = Mathf.Clamp(texY, 0, sprite.height - 1);
                Color32 c = sprite.GetPixel(texX, texY);
                if (c.a > 150)
                    pixels[y * screenWidth + x] = c;
            }
        }
    }
}