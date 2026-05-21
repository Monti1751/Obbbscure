using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    public Texture2D gameOverImage;
    public Texture2D victoryImage;

    public RawImage fullscreenImage;
    public TextMeshProUGUI restartText;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private bool gameEnded = false;

    void Awake() { instance = this; }

    void Update()
    {
        if (gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.R))
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            return;
        }

        // Comprobar Game Over
        if (GameState.instance != null && GameState.instance.health <= 0)
            TriggerGameOver();
    }

    public void TriggerGameOver()
    {
        if (gameEnded) return;
        gameEnded = true;
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.StopCounting();
            ScoreManager.instance.SaveHighScore();
        }
        if (AudioManager.instance != null)
            AudioManager.instance.PlayGameOverMusic();
        ShowScreen(gameOverImage, "GAME OVER");
    }

    public void TriggerVictory()
    {
        if (gameEnded) return;
        gameEnded = true;
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddLevelPoints();
            ScoreManager.instance.StopCounting();
            ScoreManager.instance.SaveHighScore();
        }
        if (AudioManager.instance != null)
            AudioManager.instance.PlayVictoryMusic();
        ShowScreen(victoryImage, "VICTORY!");
    }

    void ShowScreen(Texture2D image, string message)
    {
        Raycaster.instance.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (fullscreenImage != null)
        {
            fullscreenImage.texture = image;
            fullscreenImage.gameObject.SetActive(true);
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + ScoreManager.instance.GetScore();
            scoreText.gameObject.SetActive(true);
        }

        if (highScoreText != null)
        {
            // GetHighScore ya tiene el nuevo récord porque SaveHighScore se llamó antes
            highScoreText.text = "Record: " + PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.gameObject.SetActive(true);
        }

        if (restartText != null)
        {
            restartText.text = "Press R to restart";
            restartText.gameObject.SetActive(true);
        }
    }

    public bool IsGameEnded() { return gameEnded; }
}