using UnityEngine;

public enum Difficulty { Easy, Normal, Hard }

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;

    public Difficulty currentDifficulty = Difficulty.Normal;

    void Awake() 
    { 
        instance = this;
        LoadDifficulty();
    }

    public void SetDifficulty(Difficulty diff)
    {
        currentDifficulty = diff;
        PlayerPrefs.SetInt("Difficulty", (int)diff);
    }

    public void LoadDifficulty()
    {
        currentDifficulty = (Difficulty)PlayerPrefs.GetInt("Difficulty", 1);
    }

    public int GetStartingAmmo()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy: return 75;
            case Difficulty.Hard: return 25;
            default: return 50;
        }
    }

    public float GetEnemySpeed()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy: return 0.8f;
            case Difficulty.Hard: return 2.2f;
            default: return 1.5f;
        }
    }
}