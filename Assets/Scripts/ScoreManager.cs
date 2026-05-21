using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private int score = 0;
    private float levelTime = 0f;
    private bool counting = true;

    // Puntos
    private int pointsPerKill = 100;
    private int pointsPerLevel = 500;
    private int pointsPerSecondLeft = 10;
    private float maxLevelTime = 300f; // 5 minutos por nivel

    void Awake() { instance = this; }

    void Update()
    {
        if (!counting) return;
        if (PauseMenu.instance != null && PauseMenu.instance.IsPaused()) return;
        if (GameOverManager.instance != null && GameOverManager.instance.IsGameEnded()) return;

        levelTime += Time.deltaTime;
    }

    public void AddKillPoints()
    {
        score += pointsPerKill;
    }

    public void AddLevelPoints()
    {
        score += pointsPerLevel;

        // Bonus por tiempo restante
        float timeLeft = Mathf.Max(0, maxLevelTime - levelTime);
        int timeBonus = (int)(timeLeft * pointsPerSecondLeft);
        score += timeBonus;

        // Resetear timer para el siguiente nivel
        levelTime = 0f;
    }

    public void StopCounting()
    {
        counting = false;
    }

    public int GetScore() { return score; }

    public int GetHighScore() { return PlayerPrefs.GetInt("HighScore", 0); }

    public void SaveHighScore()
    {
        if (score > GetHighScore())
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
    }

    public void Reset()
    {
        score = 0;
        levelTime = 0f;
        counting = true;
    }

    public void LoadScore(int savedScore)
    {
        score = savedScore;
    }
}