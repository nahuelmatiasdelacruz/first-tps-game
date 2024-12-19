using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{

    public static WaveManager SharedInstance;

    private List<WaveSpawner> _waves;

    private int _totalWaves;
    
    public UnityEvent onWaveChange;
    
    public int WaveCount
    {
        get => _waves.Count;
    }

    public int TotalWaves
    {
        get => _totalWaves;
    }
    
    void Awake()
    {
        if (SharedInstance == null)
        {
            SharedInstance = this;
            _waves = new List<WaveSpawner>();
        }
        else
        {
            Debug.LogWarning("There is another instance of WaveManager in the game");
            Destroy(this);
        }
    }

    public void AddWave(WaveSpawner wave)
    {
        _totalWaves++;
        _waves.Add(wave);
        onWaveChange.Invoke();
    }

    public void RemoveWave(WaveSpawner wave)
    {
        _waves.Remove(wave);
        onWaveChange.Invoke();
    }
}
