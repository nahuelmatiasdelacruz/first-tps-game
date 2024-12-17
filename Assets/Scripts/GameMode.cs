using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    [SerializeField]
    private Life playerLife;
    void Update()
    {
        if (EnemyManager.SharedInstance.enemies.Count <= 0 && WaveManager.SharedInstance.waves.Count <= 0)
        {
            SceneManager.LoadScene("WinScene",LoadSceneMode.Single);
        }

        if (playerLife.Amount <= 0)
        {
            SceneManager.LoadScene("LoseScene", LoadSceneMode.Single);
        }
    }
}
