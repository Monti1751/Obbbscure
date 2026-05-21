using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    public static WeaponAnimator instance;

    public Texture2D[] idleFrames;   // 1 frame: pistol_idle
    public Texture2D[] fireFrames;   // 3 frames: fire1, fire2, fire3

    private bool isFiring = false;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private float frameInterval = 0.07f; // segundos entre frames

    void Awake() { instance = this; }

    void Update()
    {
        if (isFiring)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameInterval)
            {
                frameTimer = 0f;
                currentFrame++;
                if (currentFrame >= fireFrames.Length)
                {
                    isFiring = false;
                    currentFrame = 0;
                }
            }
        }
    }

    public void PlayFireAnimation()
    {
        if (!isFiring)
        {
            isFiring = true;
            currentFrame = 0;
            frameTimer = 0f;
        }
    }

    public Texture2D GetCurrentFrame()
    {
        if (isFiring)
            return fireFrames[Mathf.Clamp(currentFrame, 0, fireFrames.Length - 1)];
        return idleFrames[0];
    }
}