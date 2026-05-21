using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public Texture2D menuImage;
    public RawImage backgroundImage;
    private Difficulty selectedDifficulty;

    [Header("Panel Principal")]
    public GameObject mainPanel;
    public Button newGameButton;
    public Button continueButton;
    public Button quitButton;

    [Header("Panel Dificultad")]
    public GameObject difficultyPanel;
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;

    [Header("Panel Controles")]
    public GameObject controlsPanel;
    public Button classicButton;
    public Button modernButton;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (backgroundImage != null && menuImage != null)
            backgroundImage.texture = menuImage;

        mainPanel.SetActive(true);
        difficultyPanel.SetActive(false);
        controlsPanel.SetActive(false);

        newGameButton.onClick.AddListener(ShowDifficulty);
        continueButton.onClick.AddListener(Continue);
        quitButton.onClick.AddListener(Quit);

        easyButton.onClick.AddListener(() => ShowControls(Difficulty.Easy));
        normalButton.onClick.AddListener(() => ShowControls(Difficulty.Normal));
        hardButton.onClick.AddListener(() => ShowControls(Difficulty.Hard));

        classicButton.onClick.AddListener(() => StartNewGame(false));
        modernButton.onClick.AddListener(() => StartNewGame(true));

        continueButton.interactable = PlayerPrefs.HasKey("SavedGame");

        if (DifficultyManager.instance != null)
            DifficultyManager.instance.LoadDifficulty();
    }

    void ShowDifficulty()
    {
        mainPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    void ShowControls(Difficulty diff)
    {
        selectedDifficulty = diff;
        difficultyPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    void StartNewGame(bool modernControls)
    {
        if (DifficultyManager.instance != null)
            DifficultyManager.instance.SetDifficulty(selectedDifficulty);

        int savedDifficulty = (int)selectedDifficulty;
        int highScore = PlayerPrefs.GetInt("HighScore", 0); // Guardar récord
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("HighScore", highScore); // Restaurar récord
        PlayerPrefs.SetInt("Difficulty", savedDifficulty);
        PlayerPrefs.SetInt("LoadSave", 0);
        PlayerPrefs.SetInt("ModernControls", modernControls ? 1 : 0);
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("Juego");
    }

    void Continue()
    {
        PlayerPrefs.SetInt("LoadSave", 1);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Juego");
    }

    void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}