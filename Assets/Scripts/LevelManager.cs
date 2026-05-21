using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private Vector2[] startPositions = new Vector2[]
    {
        new Vector2(3.5f, 3.5f),  // Nivel 1
        new Vector2(2.5f, 2.5f),  // Nivel 2
        new Vector2(2.5f, 2.5f),  // Nivel 3
        new Vector2(2.5f, 2.5f),  // Nivel 4
        new Vector2(3.5f, 15.5f), // Nivel 5 - boss
    };

    void Awake()
    {
        instance = this;

        // Solo restaurar el nivel si es una partida continuada (LoadSave == 1)
        // Si es una nueva partida (LoadSave == 0) empezamos siempre en nivel 1
        int loadSave = PlayerPrefs.GetInt("LoadSave", 0);
        if (loadSave == 1)
        {
            int savedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            MapData.currentLevel = savedLevel;
            //Debug.Log("LevelManager Awake - partida continuada, nivel restaurado: " + MapData.currentLevel);
        }
        else
        {
            MapData.currentLevel = 1;
            PlayerPrefs.DeleteKey("CurrentLevel");
            //Debug.Log("LevelManager Awake - nueva partida, nivel: 1");
        }
    }

    void Start()
    {
        // En Awake solo seteamos el nivel, en Start lo spawneamos para el nivel 1
        // o el que hayamos restaurado. Si ya se encarga otra cosa, esto refuerza que
        // las decoraciones empiecen spawneadas.
        if (DecorationManager.instance != null)
        {
            DecorationManager.instance.SpawnDecorationsForLevel(MapData.currentLevel);
        }

        // Marcar dificultad actual
        if (DifficultyManager.instance != null)
            DifficultyManager.instance.LoadDifficulty();
    }

    void Update()
    {
        if (Raycaster.instance == null) return;
        if (GameOverManager.instance != null && GameOverManager.instance.IsGameEnded()) return;
    }

    public void LoadNextLevel()
    {
        if (ScoreManager.instance != null)
            ScoreManager.instance.AddLevelPoints();
        if (AudioManager.instance != null)
            AudioManager.instance.PlayNextLevel();

        MapData.currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", MapData.currentLevel);
        PlayerPrefs.Save();
        MapData.Reset();

        EnemyManager.instance.SpawnEnemiesForLevel(MapData.currentLevel);
        PickupManager.instance.SpawnPickupsForLevel(MapData.currentLevel);

        if (DecorationManager.instance != null)
            DecorationManager.instance.SpawnDecorationsForLevel(MapData.currentLevel);

        int levelIndex = MapData.currentLevel - 1;
        Raycaster.instance.pos = startPositions[levelIndex];
        Raycaster.instance.dir = new Vector2(1, 0);
        Raycaster.instance.plane = new Vector2(0, 0.66f);
    }
}