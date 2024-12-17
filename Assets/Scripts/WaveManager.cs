using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public static WaveManager SharedInstance;

    public List<WaveSpawner> waves;
    
    void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
            waves = new List<WaveSpawner>();
        }
        else
        {
            Debug.LogWarning("There is another instance of WaveManager in the game");
            Destroy(this);
        }
    }
}
