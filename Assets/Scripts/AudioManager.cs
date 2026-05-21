using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Efectos de sonido")]
    public AudioClip shootSound;
    public AudioClip footstepSound;
    public AudioClip doorSound;
    public AudioClip enemyDieSound;
    public AudioClip pickupSound;
    public AudioClip enemyAlertSound;

    [Header("Música")]
    public AudioClip backgroundMusic;

    private AudioSource sfxSource;
    private AudioSource footstepSource;
    private AudioSource musicSource;

    private float footstepTimer = 0f;
    private float footstepInterval = 0.4f;
    private bool isMoving = false;

    void Awake()
    {
        instance = this;

        // Crear tres AudioSources separados
        sfxSource = gameObject.AddComponent<AudioSource>();
        footstepSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        // Configurar música
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = 0.4f;
        musicSource.Play();
    }

    public void PlayShoot()
    {
        sfxSource.PlayOneShot(shootSound);
    }

    public void PlayDoor()
    {
        sfxSource.PlayOneShot(doorSound);
    }

    public void PlayEnemyDie()
    {
        sfxSource.PlayOneShot(enemyDieSound);
    }

    public void PlayPickup()
    {
        sfxSource.PlayOneShot(pickupSound);
    }

    public void PlayEnemyAlert()
    {
        sfxSource.PlayOneShot(enemyAlertSound);
    }

    public void UpdateFootsteps(bool moving)
    {
        isMoving = moving;

        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                footstepSource.PlayOneShot(footstepSound, 0.6f);
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    [Header("Música de estado")]
    public AudioClip gameOverMusic;
    public AudioClip victoryMusic;
    public AudioClip damageSound;
    public AudioClip nextLevelSound;

    public void PlayGameOverMusic()
    {
        musicSource.Stop();
        musicSource.loop = false;
        musicSource.clip = gameOverMusic;
        musicSource.volume = 0.6f;
        musicSource.Play();
    }

    public void PlayVictoryMusic()
    {
        musicSource.Stop();
        musicSource.loop = false;
        musicSource.clip = victoryMusic;
        musicSource.volume = 0.6f;
        musicSource.Play();
    }
    
    public void PlayDamage()
    {
        sfxSource.PlayOneShot(damageSound);
    }

    public void PlayNextLevel()
    {
        musicSource.Stop();
        musicSource.loop = false;
        musicSource.clip = nextLevelSound;
        musicSource.volume = 0.6f;
        musicSource.Play();

        // Reanudar música de fondo cuando termine el sonido de nivel
        Invoke("ResumeBackgroundMusic", nextLevelSound.length);
    }

    void ResumeBackgroundMusic()
    {
        musicSource.loop = true;
        musicSource.clip = backgroundMusic;
        musicSource.volume = 0.4f;
        musicSource.Play();
    }
}