using UnityEngine;
using System.Collections.Generic;

public class PickupManager : MonoBehaviour
{
    public static PickupManager instance;
    public Texture2D healthSprite;
    public Texture2D ammoSprite;
    public List<Pickup> pickups = new List<Pickup>();

    void Awake() { instance = this; }

    void Start()
    {
        SpawnPickupsForLevel(MapData.currentLevel);
    }

    public void SpawnPickupsForLevel(int level)
    {
        pickups.Clear();
        switch (level)
        {
            case 1:
                pickups.Add(new Pickup(new Vector2(3.5f, 8.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(10.5f, 5.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(15.5f, 10.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(5.5f, 15.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(20.5f, 8.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(8.5f, 20.5f), PickupType.Ammo));
                break;
            case 2:
                pickups.Add(new Pickup(new Vector2(5.5f, 5.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(15.5f, 5.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(8.5f, 12.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(20.5f, 12.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(5.5f, 20.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(22.5f, 20.5f), PickupType.Ammo));
                break;
            case 3:
                pickups.Add(new Pickup(new Vector2(5.5f, 5.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(15.5f, 5.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(25.5f, 5.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(8.5f, 12.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(18.5f, 12.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(28.5f, 12.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(5.5f, 20.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(15.5f, 20.5f), PickupType.Ammo));
                break;
            case 4:
                pickups.Add(new Pickup(new Vector2(5.5f, 3.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(14.5f, 3.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(22.5f, 3.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(7.5f, 15.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(14.5f, 19.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(21.5f, 15.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(3.5f, 21.5f), PickupType.Ammo));
                break;
            case 5:
                pickups.Add(new Pickup(new Vector2(5.5f, 15.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(11.5f, 15.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(18.5f, 15.5f), PickupType.Health));
                pickups.Add(new Pickup(new Vector2(11.5f, 4.5f), PickupType.Ammo));
                pickups.Add(new Pickup(new Vector2(11.5f, 12.5f), PickupType.Health));
                break;
        }
    }

    void Update()
    {
        if (Raycaster.instance == null) return;
        Vector2 playerPos = Raycaster.instance.pos;
        foreach (var pickup in pickups)
        {
            if (pickup.collected) continue;
            
            if (Vector2.Distance(playerPos, pickup.pos) < 0.6f)
            {
                if (pickup.type == PickupType.Health && GameState.instance.health >= 100)
                {
                    continue; // Skip collection if health is already full
                }

                Collect(pickup);
            }
        }
    }

    void Collect(Pickup pickup)
    {
        pickup.collected = true;
        if (pickup.type == PickupType.Health)
            GameState.instance.Heal(pickup.healthAmount);
        else
            GameState.instance.AddAmmo(pickup.ammoAmount);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayPickup();
    }

    public void DrawPickup(Pickup pickup, Color32[] pixels, int screenWidth, int screenHeight,
                           Vector2 playerPos, Vector2 playerDir, Vector2 playerPlane, float[] zBuffer)
    {
        if (pickup.collected) return;
        Texture2D sprite = pickup.type == PickupType.Health ? healthSprite : ammoSprite;
        if (sprite == null) return;
        Vector2 relPos = pickup.pos - playerPos;
        float invDet = 1f / (playerPlane.x * playerDir.y - playerDir.x * playerPlane.y);
        float transformX = invDet * (playerDir.y * relPos.x - playerDir.x * relPos.y);
        float transformY = invDet * (-playerPlane.y * relPos.x + playerPlane.x * relPos.y);
        if (transformY <= 0.1f) return;
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
                if (c.a > 128)
                    pixels[y * screenWidth + x] = c;
            }
        }
    }
}