using UnityEngine;
using TMPro;

public class GameState : MonoBehaviour
{
    [Header("Valores del jugador")]
    public int health = 100;
    public int ammo = 50;

    [Header("Referencias HUD")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI ammoText;

    public static GameState instance;
    public TextMeshProUGUI saveMessageText;
    private float saveMessageTimer = 0f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        MapData.Reset();

        int loadSave = PlayerPrefs.GetInt("LoadSave", 0);
        if (loadSave == 1)
            Invoke("LoadGame", 0.1f); // pequeño delay para que EnemyManager se inicialice
        else
        {
            health = 100;
            ammo = DifficultyManager.instance != null
            ? DifficultyManager.instance.GetStartingAmmo()
            : 50;
        }
    }
    void Update()
    {
        // Vornieado manual con F5
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
            saveMessageTimer = 3f;
            if (saveMessageText != null)
                saveMessageText.gameObject.SetActive(true);
        }

        // Ocultar mensaje despu�s de 3 segundos
        if (saveMessageTimer > 0f)
        {
            saveMessageTimer -= Time.deltaTime;
            if (saveMessageTimer <= 0f)
            {
                if (saveMessageText != null)
                    saveMessageText.gameObject.SetActive(false);
            }
        }

        // Actualiza el HUD cada frame
        healthText.text = "HEALTH: " + health;
        ammoText.text = "AMMO: " + ammo;

        // Muerte
        if (health <= 0)
        {
            health = 0;
            ////Debug.Log("Has muerto!");
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (AudioManager.instance != null)
            AudioManager.instance.PlayDamage();

        if (health <= 0)
        {
            health = 0;
            if (GameOverManager.instance != null)
                GameOverManager.instance.TriggerGameOver();
        }
    }

    public void UseAmmo(int amount)
    {
        if (ammo > 0) ammo -= amount;
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(100, health + amount);
    }

    public void AddAmmo(int amount)
    {
        ammo += amount;
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("SavedGame", 1);
        PlayerPrefs.SetInt("Health", health);
        PlayerPrefs.SetInt("Ammo", ammo);

        // Posición y dirección
        PlayerPrefs.SetFloat("PosX", Raycaster.instance.pos.x);
        PlayerPrefs.SetFloat("PosY", Raycaster.instance.pos.y);
        PlayerPrefs.SetFloat("DirX", Raycaster.instance.dir.x);
        PlayerPrefs.SetFloat("DirY", Raycaster.instance.dir.y);
        PlayerPrefs.SetFloat("PlaneX", Raycaster.instance.plane.x);
        PlayerPrefs.SetFloat("PlaneY", Raycaster.instance.plane.y);

        // Score
        if (ScoreManager.instance != null)
            PlayerPrefs.SetInt("Score", ScoreManager.instance.GetScore());

        // Puertas
        string doorData = "";
        foreach (var key in MapData.GetDoorStates())
            doorData += key.Key.Item1 + "," + key.Key.Item2 + "," + key.Value + ";";
        PlayerPrefs.SetString("Doors", doorData);

        // Enemigos muertos
        string enemyData = "";
        for (int i = 0; i < EnemyManager.instance.enemies.Count; i++)
            if (!EnemyManager.instance.enemies[i].alive)
                enemyData += i + ";";
        PlayerPrefs.SetString("DeadEnemies", enemyData);

        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SavedGame")) return;

        health = PlayerPrefs.GetInt("Health", 100);
        ammo = PlayerPrefs.GetInt("Ammo", 50);

        // Posición y dirección
        float px = PlayerPrefs.GetFloat("PosX", 3.5f);
        float py = PlayerPrefs.GetFloat("PosY", 3.5f);
        float dx = PlayerPrefs.GetFloat("DirX", 1f);
        float dy = PlayerPrefs.GetFloat("DirY", 0f);
        float plx = PlayerPrefs.GetFloat("PlaneX", 0f);
        float ply = PlayerPrefs.GetFloat("PlaneY", 0.66f);

        Raycaster.instance.pos = new Vector2(px, py);
        Raycaster.instance.dir = new Vector2(dx, dy);
        Raycaster.instance.plane = new Vector2(plx, ply);

        // Score
        if (ScoreManager.instance != null)
            ScoreManager.instance.LoadScore(PlayerPrefs.GetInt("Score", 0));

        // Puertas
        string doorData = PlayerPrefs.GetString("Doors", "");
        if (doorData != "")
        {
            foreach (string entry in doorData.Split(';'))
            {
                if (entry == "") continue;
                string[] parts = entry.Split(',');
                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);
                float val = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                MapData.SetDoorState(x, y, val);
            }
        }

        // Enemigos muertos
        string enemyData = PlayerPrefs.GetString("DeadEnemies", "");
        if (enemyData != "")
        {
            foreach (string entry in enemyData.Split(';'))
            {
                if (entry == "") continue;
                int i = int.Parse(entry);
                if (i < EnemyManager.instance.enemies.Count)
                    EnemyManager.instance.enemies[i].alive = false;
            }
        }
    }
}
