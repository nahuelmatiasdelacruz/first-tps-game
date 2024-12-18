using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    [SerializeField]
    private Life playerLife;

    [SerializeField]
    private Life baseLife;
    
    void Awake()
    {
        playerLife.onDeath.AddListener(CheckLoseCondition);
        baseLife.onDeath.AddListener(CheckLoseCondition);
        
        EnemyManager.SharedInstance.onEnemyAmountChange.AddListener(CheckWinCondition);
        WaveManager.SharedInstance.onWaveChange.AddListener(CheckWinCondition);
    }

    void CheckLoseCondition()
    {
        SceneManager.LoadScene("LoseScene", LoadSceneMode.Single);
    }

    void CheckWinCondition()
    {
        if (EnemyManager.SharedInstance.EnemyCount<= 0 && WaveManager.SharedInstance.WaveCount <= 0)
        {
            SceneManager.LoadScene("WinScene",LoadSceneMode.Single);
        }
    }
}
