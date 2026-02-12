using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject teleporter;
    public Transform enemyGroup; // Drag the folder/parent of Room 1 enemies here
    public float checkInterval = 0.5f;

    void Start()
    {
        if (teleporter != null) teleporter.SetActive(false);
        InvokeRepeating("CheckEnemies", 1f, checkInterval);
    }

    void CheckEnemies()
    {
        // Checks if the Room 1 enemy group has any children left
        if (enemyGroup != null && enemyGroup.childCount == 0)
        {
            if (teleporter != null && !teleporter.activeSelf)
            {
                teleporter.SetActive(true);
                CancelInvoke("CheckEnemies");
            }
        }
    }
}