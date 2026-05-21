using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;

    public RawImage faceImage;
    public Texture2D faceHealthy;
    public Texture2D faceHurt;
    public Texture2D faceDying;

    void Awake() { instance = this; }

    void Update()
    {
        if (GameState.instance == null || faceImage == null) return;

        int health = GameState.instance.health;

        if (health >= 60)
            faceImage.texture = faceHealthy;
        else if (health >= 30)
            faceImage.texture = faceHurt;
        else
            faceImage.texture = faceDying;
    }
}
