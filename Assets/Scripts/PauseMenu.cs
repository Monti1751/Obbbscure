using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    public GameObject pausePanel;
    public GameObject controlsPanel;

    public Button resumeButton;
    public Button controlsButton;
    public Button quitButton;
    public Button backButton;

    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;

    private bool isPaused = false;

    void Awake() { instance = this; }

    void Start()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);

        resumeButton.onClick.AddListener(Resume);
        controlsButton.onClick.AddListener(ShowControls);
        quitButton.onClick.AddListener(Quit);
        backButton.onClick.AddListener(HideControls);

        // Configurar slider
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.7f);
        AudioListener.volume = volumeSlider.value;
        UpdateVolumeText();

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        UpdateVolumeText();
    }

    void UpdateVolumeText()
    {
        if (volumeText != null)
            volumeText.text = "Volume: " + (int)(volumeSlider.value * 100) + "%";
    }

    void Update()
    {
        if (GameOverManager.instance != null && GameOverManager.instance.IsGameEnded()) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (controlsPanel.activeSelf)
                HideControls();
            else if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (AudioManager.instance != null)
            AudioManager.instance.PauseMusic();
    }

    void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        controlsPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (AudioManager.instance != null)
            AudioManager.instance.ResumeMusic();
    }

    void ShowControls()
    {
        pausePanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    void HideControls()
    {
        controlsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    void Quit()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public bool IsPaused() { return isPaused; }
}