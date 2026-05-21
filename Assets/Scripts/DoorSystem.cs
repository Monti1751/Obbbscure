using UnityEngine;

public class DoorSystem : MonoBehaviour
{
    void Update()
    {
        if (Raycaster.instance == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 playerPos = Raycaster.instance.pos;
            Vector2 playerDir = Raycaster.instance.dir;

            for (float dist = 0.3f; dist <= 1.5f; dist += 0.2f)
            {
                int checkX = (int)(playerPos.x + playerDir.x * dist);
                int checkY = (int)(playerPos.y + playerDir.y * dist);

                //Debug.Log($"({checkX},{checkY}) = {MapData.GetWallType(checkX, checkY)}");

                // Comprobar palanca
                if (MapData.IsLever(checkX, checkY))
                {
                    //Debug.Log("PALANCA - LevelManager: " + LevelManager.instance + " nivel: " + MapData.currentLevel);
                    if (MapData.currentLevel < 5)
                    {
                        if (LevelManager.instance != null)
                            LevelManager.instance.LoadNextLevel();
                        //else
                            //Debug.Log("LevelManager es NULL");
                    }
                    else
                    {
                        if (GameOverManager.instance != null)
                            GameOverManager.instance.TriggerVictory();
                        //else
                            //Debug.Log("GameOverManager es NULL");
                    }
                    return;
                }

                // Comprobar puerta
                if (MapData.IsDoor(checkX, checkY))
                {
                    MapData.ActivateDoor(checkX, checkY);
                    if (AudioManager.instance != null)
                        AudioManager.instance.PlayDoor();
                    return;
                }
            }
        }
    }
}