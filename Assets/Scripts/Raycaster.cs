using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Raycaster : MonoBehaviour
{
    // 1. EL SINGLETON (Arregla los errores de DoorSystem, Weapon, etc.)
    public static Raycaster instance;
    private bool showMinimap = false;

    [Header("UI Interaccion")]
    public TextMeshProUGUI interactText;

    [Header("Configuracion de Pantalla")]
    public RawImage display;
    public int screenWidth = 640;
    public int screenHeight = 400;

    [Header("Texturas")]
    public Texture2D[] wallTextures;

    [Header("Jugador")]
    public Vector2 pos = new Vector2(3.5f, 3.5f);
    public Vector2 dir = new Vector2(1, 0);
    public Vector2 plane = new Vector2(0, 0.66f);

    private Texture2D screenTexture;
    private Color32[] pixels;
    private float[] zBuffer;

    void Awake()
    {
        // Inicializamos la instancia antes que nada
        instance = this;
    }

    void Start()
    {
        // Bloqueamos el cursor para que no moleste al jugar
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Configuramos la textura para que sea nitida y no deje rastro
        screenTexture = new Texture2D(screenWidth, screenHeight, TextureFormat.RGBA32, false);
        screenTexture.filterMode = FilterMode.Point;
        screenTexture.wrapMode = TextureWrapMode.Clamp;
        display.texture = screenTexture;

        pixels = new Color32[screenWidth * screenHeight];
        zBuffer = new float[screenWidth];
    }

    void Update()
    {
        // Confirmacion para la consola
        //Debug.Log("EL SCRIPT ESTA VIVO");

        MapData.UpdateDoors();
        HandleInput();
        DrawFrame();
        CheckInteractionUI();
    }

    void CheckInteractionUI()
    {
        if (interactText == null) return;

        int targetX = (int)(pos.x + dir.x * 0.5f);
        int targetY = (int)(pos.y + dir.y * 0.5f);

        int tileType = MapData.GetWallType(targetX, targetY);
        bool showInteract = false;

        // Tile 2 es puerta, Tile 6 es pasar de nivel
        if (tileType == 2)
        {
            // Solo mostrar si la puerta esta casi cerrada
            if (MapData.GetDoorOpen(targetX, targetY) < 0.1f)
            {
                showInteract = true;
            }
        }
        else if (tileType == 6)
        {
            showInteract = true;
        }

        if (interactText.gameObject.activeSelf != showInteract)
        {
            interactText.gameObject.SetActive(showInteract);
        }
    }

    void DrawFrame()
    {
        // 1. LIMPIEZA DE FONDO (Elimina el glitch del video)
        Color32 sky = new Color32(40, 40, 50, 255);
        Color32 ground = new Color32(60, 60, 60, 255);

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = (i < (screenWidth * screenHeight) / 2) ? ground : sky;
        }

        // 2. BUCLE RAYCASTING
        for (int x = 0; x < screenWidth; x++)
        {
            float cameraX = 2f * x / (float)screenWidth - 1f;
            Vector2 rayDir = dir + (plane * cameraX);

            int mapX = (int)pos.x;
            int mapY = (int)pos.y;

            float deltaX = Mathf.Abs(1f / (rayDir.x == 0 ? 1e-30f : rayDir.x));
            float deltaY = Mathf.Abs(1f / (rayDir.y == 0 ? 1e-30f : rayDir.y));

            int stepX = (rayDir.x < 0) ? -1 : 1;
            int stepY = (rayDir.y < 0) ? -1 : 1;

            float sideX = (rayDir.x < 0) ? (pos.x - mapX) * deltaX : (mapX + 1.0f - pos.x) * deltaX;
            // CORRECCION LINEA 71: Cambiado '?' por '*'
            float sideY = (rayDir.y < 0) ? (pos.y - mapY) * deltaY : (mapY + 1.0f - pos.y) * deltaY;

            bool hit = false;
            int side = 0;
            int tile = 0;
            float wallDist = 0;
            float wallX = 0;

            int depth = 0;
            while (!hit && depth < 64)
            {
                depth++;
                if (sideX < sideY) { sideX += deltaX; mapX += stepX; side = 0; }
                else { sideY += deltaY; mapY += stepY; side = 1; }

                tile = MapData.GetWallType(mapX, mapY);

                if (tile == 2) // Puerta
                {
                    float dDist = (side == 0) ? (sideX - deltaX * 0.5f) : (sideY - deltaY * 0.5f);
                    bool isInside = (side == 0) ? (dDist < sideY) : (dDist < sideX);

                    if (isInside)
                    {
                        float hX = (side == 0) ? pos.y + dDist * rayDir.y : pos.x + dDist * rayDir.x;
                        hX -= Mathf.Floor(hX);

                        if (hX > MapData.GetDoorOpen(mapX, mapY))
                        {
                            hit = true;
                            wallDist = dDist;
                            wallX = hX - MapData.GetDoorOpen(mapX, mapY);
                        }
                    }
                }
                else if (tile > 0) // Muro solido
                {
                    hit = true;
                    wallDist = (side == 0) ? (sideX - deltaX) : (sideY - deltaY);
                    wallX = (side == 0) ? pos.y + wallDist * rayDir.y : pos.x + wallDist * rayDir.x;
                    wallX -= Mathf.Floor(wallX);
                }
            }

            // 3. DIBUJO
            if (hit && wallDist > 0)
            {
                zBuffer[x] = wallDist;
                int lineH = (int)(screenHeight / wallDist);
                int dStart = Mathf.Max(0, -lineH / 2 + screenHeight / 2);
                int dEnd = Mathf.Min(screenHeight - 1, lineH / 2 + screenHeight / 2);

                Texture2D tex = wallTextures[Mathf.Clamp(tile - 1, 0, wallTextures.Length - 1)];
                int texX = (int)(wallX * tex.width);
                if ((side == 0 && rayDir.x > 0) || (side == 1 && rayDir.y < 0)) texX = tex.width - texX - 1;

                for (int y = dStart; y < dEnd; y++)
                {
                    int d = y * 256 - screenHeight * 128 + lineH * 128;
                    int texY = ((d * tex.height) / lineH) / 256;
                    Color32 c = tex.GetPixel(texX, Mathf.Clamp(texY, 0, tex.height - 1));
                    if (side == 1) { c.r >>= 1; c.g >>= 1; c.b >>= 1; }
                    pixels[y * screenWidth + x] = c;
                }
            }
        }

        // Ordenar todos los sprites juntos por distancia
        var spritesToDraw = new List<(float dist, System.Action draw)>();

        if (EnemyManager.instance != null)
            foreach (var e in EnemyManager.instance.enemies)
                if (e.alive || e.animState == Enemy.AnimState.Die)
                {
                    var enemy = e;
                    spritesToDraw.Add((Vector2.Distance(e.pos, pos),
                        () => EnemyManager.instance.DrawEnemy(enemy, pixels, screenWidth, screenHeight, pos, dir, plane, zBuffer)));
                }

        if (PickupManager.instance != null)
            foreach (var p in PickupManager.instance.pickups)
                if (!p.collected)
                {
                    var pickup = p;
                    spritesToDraw.Add((Vector2.Distance(p.pos, pos),
                        () => PickupManager.instance.DrawPickup(pickup, pixels, screenWidth, screenHeight, pos, dir, plane, zBuffer)));
                }

        if (DecorationManager.instance != null)
            foreach (var d in DecorationManager.instance.decorations)
            {
                var dec = d;
                spritesToDraw.Add((Vector2.Distance(d.pos, pos),
                    () => DecorationManager.instance.DrawDecoration(dec, pixels, screenWidth, screenHeight, pos, dir, plane, zBuffer)));
            }

        if (ProjectileManager.instance != null)
            foreach (var p in ProjectileManager.instance.projectiles)
                if (p.active)
                {
                    var proj = p;
                    spritesToDraw.Add((Vector2.Distance(p.pos, pos),
                        () => ProjectileManager.instance.DrawProjectile(proj, pixels, screenWidth, screenHeight, pos, dir, plane, zBuffer)));
                }

        // Dibujar de lejos a cerca
        spritesToDraw.Sort((a, b) => b.dist.CompareTo(a.dist));
        foreach (var s in spritesToDraw) s.draw();

        // Arma siempre encima
        if (WeaponAnimator.instance != null)
            DrawWeapon(pixels);

        DrawMinimap(pixels);
        screenTexture.SetPixels32(pixels);
        screenTexture.Apply(false);
    }

    void HandleInput()
    {
        if (PauseMenu.instance != null && PauseMenu.instance.IsPaused()) return;
        if (GameOverManager.instance != null && GameOverManager.instance.IsGameEnded()) return;

        bool moving = false;
        bool modernControls = PlayerPrefs.GetInt("ModernControls", 0) == 1;

        float mS = 5f * Time.deltaTime;
        float rS = 1.25f * Time.deltaTime;

        if (Input.GetKey(KeyCode.W)) { Move(dir * mS); moving = true; }
        if (Input.GetKey(KeyCode.S)) { Move(-dir * mS); moving = true; }

        if (modernControls)
        {
            float mouseX = Input.GetAxis("Mouse X");
            if (Mathf.Abs(mouseX) > 0.01f)
                Rotate(mouseX * 1f * Time.deltaTime);

            // Movimiento lateral
            Vector2 right = new Vector2(-dir.y, dir.x);
            if (Input.GetKey(KeyCode.D)) { Move(right * mS); moving = true; }
            if (Input.GetKey(KeyCode.A)) { Move(-right * mS); moving = true; }
        }
        else
        {
            if (Input.GetKey(KeyCode.D)) Rotate(rS);
            if (Input.GetKey(KeyCode.A)) Rotate(-rS);
        }

        if (AudioManager.instance != null)
            AudioManager.instance.UpdateFootsteps(moving);

        if (Input.GetKeyDown(KeyCode.E))
            MapData.ActivateDoor((int)(pos.x + dir.x * 0.5f), (int)(pos.y + dir.y * 0.5f));

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M))
            showMinimap = !showMinimap;
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (MapData.currentLevel < 5)
                LevelManager.instance.LoadNextLevel();
            else
                GameOverManager.instance.TriggerVictory();
        }
#endif
    }

    void Move(Vector2 d) 
    { 
        float nextX = pos.x + d.x;
        float nextY = pos.y + d.y;
        
        if (!MapData.IsWall((int)nextX, (int)pos.y) && !IsSolidDecoration(nextX, pos.y)) 
            pos.x = nextX; 
            
        if (!MapData.IsWall((int)pos.x, (int)nextY) && !IsSolidDecoration(pos.x, nextY)) 
            pos.y = nextY; 
    }

    bool IsSolidDecoration(float targetX, float targetY)
    {
        if (DecorationManager.instance == null) return false;
        
        foreach (var dec in DecorationManager.instance.decorations)
        {
            if (dec.type == DecorationType.Barrel || dec.type == DecorationType.Plant)
            {
                // Simple distance check (radius 0.4 ensures collision doesn't feel too big/small)
                if (Vector2.Distance(new Vector2(targetX, targetY), dec.pos) < 0.4f)
                {
                    return true;
                }
            }
        }
        return false;
    }
    void Rotate(float a) { float oldX = dir.x; dir.x = dir.x * Mathf.Cos(a) - dir.y * Mathf.Sin(a); dir.y = oldX * Mathf.Sin(a) + dir.y * Mathf.Cos(a); float oldPX = plane.x; plane.x = plane.x * Mathf.Cos(a) - plane.y * Mathf.Sin(a); plane.y = oldPX * Mathf.Sin(a) + plane.y * Mathf.Cos(a); }

    void DrawWeapon(Color32[] pixels)
    {
        Texture2D weaponTex = WeaponAnimator.instance.GetCurrentFrame();
        if (weaponTex == null) return;

        int weaponWidth = screenWidth / 2;
        int weaponHeight = screenWidth / 2;

        int startX = (screenWidth - weaponWidth) / 2;
        int startY = screenHeight - weaponHeight;

        for (int x = 0; x < weaponWidth; x++)
        {
            for (int y = 0; y < weaponHeight; y++)
            {
                int texX = (int)((float)x / weaponWidth * weaponTex.width);
                int texY = (int)((float)y / weaponHeight * weaponTex.height);

                Color32 c = weaponTex.GetPixel(texX, texY);
                if (c.a > 128)
                {
                    int screenX = startX + x;
                    int screenY = startY + y;
                    if (screenX >= 0 && screenX < screenWidth && screenY >= 0 && screenY < screenHeight)
                        pixels[screenY * screenWidth + screenX] = c;
                }
            }
        }
    }

    void DrawMinimap(Color32[] pixels)
    {
        if (!showMinimap) return;

        var m = MapData.GetCurrentMap();
        int mapRows = m.GetLength(0);
        int mapCols = m.GetLength(1);
        int cellSize = 4; // píxeles por celda
        int offsetX = 5;  // margen izquierdo
        int offsetY = screenHeight - 5; // margen superior

        for (int my = 0; my < mapRows; my++)
        {
            for (int mx = 0; mx < mapCols; mx++)
            {
                int tile = m[my, mx];
                Color32 color;

                switch (tile)
                {
                    case 0: color = new Color32(50, 50, 50, 200); break; // vacío
                    case 2: color = new Color32(0, 150, 255, 200); break; // puerta
                    case 6: color = new Color32(255, 200, 0, 200); break; // palanca
                    default: color = new Color32(180, 180, 180, 200); break; // pared
                }

                // Dibujar celda
                for (int py = 0; py < cellSize; py++)
                {
                    for (int px = 0; px < cellSize; px++)
                    {
                        int screenX = offsetX + mx * cellSize + px;
                        int screenY = offsetY - my * cellSize - py;

                        if (screenX >= 0 && screenX < screenWidth && screenY >= 0 && screenY < screenHeight)
                            pixels[screenY * screenWidth + screenX] = color;
                    }
                }
            }
        }

        // Dibujar jugador como punto rojo
        int playerMapX = (int)pos.x;
        int playerMapY = (int)pos.y;

        for (int py = 0; py < cellSize; py++)
        {
            for (int px = 0; px < cellSize; px++)
            {
                int screenX = offsetX + playerMapX * cellSize + px;
                int screenY = offsetY - playerMapY * cellSize - py;

                if (screenX >= 0 && screenX < screenWidth && screenY >= 0 && screenY < screenHeight)
                    pixels[screenY * screenWidth + screenX] = new Color32(255, 0, 0, 255);
            }
        }

        // Dibujar enemigos como puntos naranjas
        if (EnemyManager.instance != null)
        {
            foreach (var enemy in EnemyManager.instance.enemies)
            {
                if (!enemy.alive) continue;
                int ex = (int)enemy.pos.x;
                int ey = (int)enemy.pos.y;

                for (int py = 0; py < cellSize; py++)
                {
                    for (int px = 0; px < cellSize; px++)
                    {
                        int screenX = offsetX + ex * cellSize + px;
                        int screenY = offsetY - ey * cellSize - py;

                        if (screenX >= 0 && screenX < screenWidth && screenY >= 0 && screenY < screenHeight)
                            pixels[screenY * screenWidth + screenX] = new Color32(255, 128, 0, 255);
                    }
                }
            }
        }
    }
}
