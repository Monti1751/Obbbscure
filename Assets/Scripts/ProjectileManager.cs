using UnityEngine;
using System.Collections.Generic;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager instance;
    public List<Projectile> projectiles = new List<Projectile>();
    public Texture2D projectileSprite;

    void Awake() { instance = this; }

    void FixedUpdate()
    {
        if (GameOverManager.instance != null && GameOverManager.instance.IsGameEnded()) return;

        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            projectiles[i].Update();
            if (!projectiles[i].active)
                projectiles.RemoveAt(i);
        }
    }

    public void SpawnProjectile(Vector2 pos, Vector2 dir)
    {
        projectiles.Add(new Projectile(pos, dir));
    }

    public void DrawProjectile(Projectile proj, Color32[] pixels, int screenWidth, int screenHeight,
                               Vector2 playerPos, Vector2 playerDir, Vector2 playerPlane, float[] zBuffer)
    {
        if (projectileSprite == null) return;

        Vector2 relPos = proj.pos - playerPos;
        float invDet = 1f / (playerPlane.x * playerDir.y - playerDir.x * playerPlane.y);
        float transformX = invDet * (playerDir.y * relPos.x - playerDir.x * relPos.y);
        float transformY = invDet * (-playerPlane.y * relPos.x + playerPlane.x * relPos.y);
        if (transformY <= 0) return;

        int spriteScreenX = (int)((screenWidth / 2) * (1 + transformX / transformY));
        int spriteHeight = Mathf.Abs((int)(screenHeight / transformY / 2));
        int spriteWidth = spriteHeight;

        int drawStartY = Mathf.Max(0, screenHeight / 2 - spriteHeight / 2);
        int drawEndY = Mathf.Min(screenHeight - 1, screenHeight / 2 + spriteHeight / 2);
        int drawStartX = Mathf.Max(0, spriteScreenX - spriteWidth / 2);
        int drawEndX = Mathf.Min(screenWidth - 1, spriteScreenX + spriteWidth / 2);

        for (int x = drawStartX; x < drawEndX; x++)
        {
            if (transformY >= zBuffer[x]) continue;
            int texX = (int)((x - (spriteScreenX - spriteWidth / 2)) * projectileSprite.width / spriteWidth);
            texX = Mathf.Clamp(texX, 0, projectileSprite.width - 1);

            for (int y = drawStartY; y < drawEndY; y++)
            {
                int texY = (int)((y - (screenHeight / 2 - spriteHeight / 2)) * projectileSprite.height / spriteHeight);
                texY = Mathf.Clamp(texY, 0, projectileSprite.height - 1);
                Color32 c = projectileSprite.GetPixel(texX, texY);
                if (c.a > 150)
                    pixels[y * screenWidth + x] = c;
            }
        }
    }
}
