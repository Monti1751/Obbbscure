using System.Collections.Generic;
using UnityEngine;

public enum DecorationType { Barrel, Lamp, Plant, Skeleton }

public class Decoration
{
    public Vector2 pos;
    public DecorationType type;

    public Decoration(Vector2 pos, DecorationType type)
    {
        this.pos = pos;
        this.type = type;
    }
}

public class DecorationManager : MonoBehaviour
{
    public static DecorationManager instance;

    [Header("Sprites de Decoracion")]
    [Tooltip("Orden: [0] Barril, [1] Lampara, [2] Planta, [3] Esqueleto")]
    public Texture2D[] decorationSprites;

    public List<Decoration> decorations = new List<Decoration>();

    void Awake() { instance = this; }

    void Start()
    {
        SpawnDecorationsForLevel(MapData.currentLevel);
    }

    public void SpawnDecorationsForLevel(int level)
    {
        decorations.Clear();
        switch (level)
        {
            case 1:
                decorations.Add(new Decoration(new Vector2(3.5f, 5.5f),   DecorationType.Barrel));  // Esquina apartada
                decorations.Add(new Decoration(new Vector2(6.5f, 6.5f),   DecorationType.Lamp));
                decorations.Add(new Decoration(new Vector2(10.5f, 2.5f),  DecorationType.Plant));   // Esquina norte
                decorations.Add(new Decoration(new Vector2(14.5f, 6.5f),  DecorationType.Skeleton));
                decorations.Add(new Decoration(new Vector2(20.5f, 13.5f), DecorationType.Barrel));  // Sala lejana, esquina
                decorations.Add(new Decoration(new Vector2(15.5f, 12.5f), DecorationType.Lamp));
                decorations.Add(new Decoration(new Vector2(5.5f, 19.5f),  DecorationType.Plant));   // Esquina sur
                decorations.Add(new Decoration(new Vector2(12.5f, 16.5f), DecorationType.Skeleton));
                decorations.Add(new Decoration(new Vector2(18.5f, 21.5f), DecorationType.Barrel));  // Esquina sureste
                decorations.Add(new Decoration(new Vector2(21.5f, 14.5f), DecorationType.Lamp));
                break;

            case 2:
                decorations.Add(new Decoration(new Vector2(5.5f, 10.5f),  DecorationType.Barrel));  // Lejos del pasillo principal
                decorations.Add(new Decoration(new Vector2(8.5f, 5.5f),   DecorationType.Lamp));
                decorations.Add(new Decoration(new Vector2(23.5f, 3.5f),  DecorationType.Plant));   // Borde norte
                decorations.Add(new Decoration(new Vector2(16.5f, 4.5f),  DecorationType.Skeleton));
                decorations.Add(new Decoration(new Vector2(23.5f, 10.5f), DecorationType.Barrel));  // Esquina este
                decorations.Add(new Decoration(new Vector2(7.5f, 13.5f),  DecorationType.Lamp));
                decorations.Add(new Decoration(new Vector2(15.5f, 15.5f), DecorationType.Plant));   // Centro amplio
                decorations.Add(new Decoration(new Vector2(20.5f, 14.5f), DecorationType.Skeleton));
                decorations.Add(new Decoration(new Vector2(5.5f, 21.5f),  DecorationType.Barrel));  // Esquina suroeste
                decorations.Add(new Decoration(new Vector2(15.5f, 21.5f), DecorationType.Lamp));
                break;

            case 3:
                decorations.Add(new Decoration(new Vector2(5.5f, 7.5f),   DecorationType.Barrel));  // Lejos de zona de paso
                decorations.Add(new Decoration(new Vector2(8.5f, 5.5f),   DecorationType.Lamp));
                decorations.Add(new Decoration(new Vector2(16.5f, 2.5f),  DecorationType.Plant));   // Borde norte
                decorations.Add(new Decoration(new Vector2(19.5f, 4.5f),  DecorationType.Skeleton));
                decorations.Add(new Decoration(new Vector2(30.5f, 7.5f),  DecorationType.Barrel));  // Extremo este
                decorations.Add(new Decoration(new Vector2(29.5f, 5.5f),  DecorationType.Lamp));
                decorations.Add(new Decoration(new Vector2(5.5f, 15.5f),  DecorationType.Plant));   // Borde oeste
                decorations.Add(new Decoration(new Vector2(14.5f, 13.5f), DecorationType.Skeleton));
                decorations.Add(new Decoration(new Vector2(30.5f, 15.5f), DecorationType.Barrel));  // Esquina este inferior
                decorations.Add(new Decoration(new Vector2(28.5f, 14.5f), DecorationType.Lamp));
                decorations.Add(new Decoration(new Vector2(5.5f, 21.5f),  DecorationType.Plant));   // Esquina suroeste
                decorations.Add(new Decoration(new Vector2(12.5f, 21.5f), DecorationType.Skeleton));
                break;
            case 4:
                decorations.Add(new Decoration(new Vector2(6.5f, 7.5f), DecorationType.Skeleton));
                decorations.Add(new Decoration(new Vector2(14.5f, 7.5f), DecorationType.Lamp));
                decorations.Add(new Decoration(new Vector2(22.5f, 7.5f), DecorationType.Plant));
                decorations.Add(new Decoration(new Vector2(6.5f, 18.5f), DecorationType.Plant));
                decorations.Add(new Decoration(new Vector2(14.5f, 18.5f), DecorationType.Barrel));
                decorations.Add(new Decoration(new Vector2(22.5f, 18.5f), DecorationType.Skeleton));
                break;
            case 5:
                decorations.Add(new Decoration(new Vector2(5.5f, 9.5f), DecorationType.Skeleton));
                decorations.Add(new Decoration(new Vector2(18.5f, 9.5f), DecorationType.Skeleton));
                decorations.Add(new Decoration(new Vector2(5.5f, 12.5f), DecorationType.Skeleton));
                decorations.Add(new Decoration(new Vector2(18.5f, 12.5f), DecorationType.Skeleton));
                break;
        }
    }

    public void DrawDecoration(Decoration dec, Color32[] pixels, int screenWidth, int screenHeight,
                               Vector2 playerPos, Vector2 playerDir, Vector2 playerPlane, float[] zBuffer)
    {
        if (decorationSprites == null || decorationSprites.Length == 0) return;

        int spriteIndex = (int)dec.type;
        if (spriteIndex >= decorationSprites.Length || decorationSprites[spriteIndex] == null) return;
        Texture2D sprite = decorationSprites[spriteIndex];

        Vector2 relPos = dec.pos - playerPos;
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
